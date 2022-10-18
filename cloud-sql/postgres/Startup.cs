﻿/*
 * Copyright (c) 2019 Google LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Polly;
using System;
using System.Data.Common;

namespace CloudSql
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add
        // services to the container.
        // For more information on how to configure your application, visit
        // http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(sp => StartupExtensions.GetPostgreSqlConnectionString());
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(DbExceptionFilterAttribute));
            });
            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Configure error reporting service.
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    static class StartupExtensions
    {
        public static void OpenWithRetry(this DbConnection connection) =>
            // [START cloud_sql_postgres_dotnet_ado_backoff]
            Policy
                .Handle<NpgsqlException>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5)
                })
                .Execute(() => connection.Open());
            // [END cloud_sql_postgres_dotnet_ado_backoff]
        public static void InitializeDatabase()
        {
            var connectionString = GetPostgreSqlConnectionString();
            using(DbConnection connection = new NpgsqlConnection(connectionString.ConnectionString))
            {
                connection.OpenWithRetry();
                using (var createTableCommand = connection.CreateCommand())
                {
                    createTableCommand.CommandText = @"
                        CREATE TABLE IF NOT EXISTS
                        votes(
                            vote_id SERIAL NOT NULL,
                            time_cast timestamp NOT NULL,
                            candidate CHAR(6) NOT NULL,
                            PRIMARY KEY (vote_id)
                        )";
                    createTableCommand.ExecuteNonQuery();
                }
            }
        }

        public static NpgsqlConnectionStringBuilder GetPostgreSqlConnectionString()
        {
            NpgsqlConnectionStringBuilder connectionString; 
            if (Environment.GetEnvironmentVariable("INSTANCE_HOST") != null)
            {
                connectionString = PostgreSqlTcp.NewPostgreSqlTCPConnectionString();
            }
            else
            {
                connectionString = PostgreSqlUnix.NewPostgreSqlUnixSocketConnectionString();
            }
            // The values set here are for demonstration purposes only. You 
            // should set these values to what works best for your application.
            // [START cloud_sql_postgres_dotnet_ado_limit]
            // MaxPoolSize sets maximum number of connections allowed in the pool.
            connectionString.MaxPoolSize = 5;
            // MinPoolSize sets the minimum number of connections in the pool.
            connectionString.MinPoolSize = 0;
            // [END cloud_sql_postgres_dotnet_ado_limit]
            // [START cloud_sql_postgres_dotnet_ado_timeout]
            // Timeout sets the time to wait (in seconds) while
            // trying to establish a connection before terminating the attempt.
            connectionString.Timeout = 15;
            // [END cloud_sql_postgres_dotnet_ado_timeout]
            // [START cloud_sql_postgres_dotnet_ado_lifetime]
            // ConnectionIdleLifetime sets the time (in seconds) to wait before
            // closing idle connections in the pool if the count of all
            // connections exceeds MinPoolSize.
            connectionString.ConnectionIdleLifetime = 300;
            // [END cloud_sql_postgres_dotnet_ado_lifetime]
            return connectionString;
        }
    }
}
