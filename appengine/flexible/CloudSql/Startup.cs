/*
 * Copyright (c) 2017 Google Inc.
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

using Google.Cloud.Diagnostics.AspNetCore;
using Google.Cloud.Diagnostics.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Net;
using System.Data.Common;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace CloudSql
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        IServiceCollection _services;

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
            string projectId = Google.Api.Gax.Platform.Instance().ProjectId;
            if (!string.IsNullOrEmpty(projectId))
            {
                services.AddGoogleExceptionLogging(options =>
                {
                    options.ProjectId = projectId;
                    options.ServiceName = "CloudSqlSample";
                    options.Version = "Test";
                });
            }
            services.AddSingleton(typeof(DbConnection), (IServiceProvider) =>
                InitializeDatabase());
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(DbExceptionFilterAttribute));
            });
            _services = services;
        }

        DbConnection InitializeDatabase()
        {
            DbConnection connection;
            string database = Configuration["CloudSQL:Database"];
            switch (database.ToLower())
            {
                case "mysql":
                    connection = NewMysqlConnection();
                    break;
                case "postgresql":
                    connection = NewPostgreSqlConnection();
                    break;
                default:
                    throw new ArgumentException(string.Format(
                        "Invalid database {0}.  Fix appsettings.json.",
                            database), "CloudSQL:Database");
            }
            connection.Open();
            using (var createTableCommand = connection.CreateCommand())
            {
                createTableCommand.CommandText = @"
                    CREATE TABLE IF NOT EXISTS
                    visits (
                        time_stamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        user_ip CHAR(64)
                    )";
                createTableCommand.ExecuteNonQuery();
            }
            return connection;
        }

        DbConnection NewMysqlConnection()
        {
            // [START gae_flex_mysql_env]
            var connectionString = new MySqlConnectionStringBuilder(
                Configuration["CloudSql:ConnectionString"])
            {
                // Connecting to a local proxy that does not support ssl.
                SslMode = MySqlSslMode.None,
            };
            DbConnection connection =
                new MySqlConnection(connectionString.ConnectionString);
            // [END gae_flex_mysql_env]
            return connection;
        }

        DbConnection NewPostgreSqlConnection()
        {
            // [START gae_flex_postgres_env]
            var connectionString = new NpgsqlConnectionStringBuilder(
                Configuration["CloudSql:ConnectionString"])
            {
                // Connecting to a local proxy that does not support ssl.
                SslMode = SslMode.Disable
            };
            NpgsqlConnection connection =
                new NpgsqlConnection(connectionString.ConnectionString);
            // [END gae_flex_postgres_env]
            return connection;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Configure error reporting service.
                app.UseGoogleExceptionLogging();
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
}
