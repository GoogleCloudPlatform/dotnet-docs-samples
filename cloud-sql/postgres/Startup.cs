/*
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

using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using System;
using System.Data;
using System.Data.Common;
using System.IO;

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
            if (Environment.GetEnvironmentVariable("DB_HOST") != null)
            {
                connectionString = NewPostgreSqlTCPConnectionString();
            }
            else
            {
                connectionString = NewPostgreSqlUnixSocketConnectionString();
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

        public static NpgsqlConnectionStringBuilder NewPostgreSqlTCPConnectionString()
        {
            // [START cloud_sql_postgres_dotnet_ado_connection_tcp]
            // Equivalent connection string:
            // "Uid=<DB_USER>;Pwd=<DB_PASS>;Host=<DB_HOST>;Database=<DB_NAME>;"
            var connectionString = new NpgsqlConnectionStringBuilder()
            {
                // The Cloud SQL proxy provides encryption between the proxy and instance.
                SslMode = SslMode.Disable,

                // Remember - storing secrets in plain text is potentially unsafe. Consider using
                // something like https://cloud.google.com/secret-manager/docs/overview to help keep
                // secrets secret.
                Host = Environment.GetEnvironmentVariable("DB_HOST"),     // e.g. '127.0.0.1'
                // Set Host to 'cloudsql' when deploying to App Engine Flexible environment
                Username = Environment.GetEnvironmentVariable("DB_USER"), // e.g. 'my-db-user'
                Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
            };
            connectionString.Pooling = true;
            // Specify additional properties here.
            return connectionString;
            // [END cloud_sql_postgres_dotnet_ado_connection_tcp]
        }

        public static NpgsqlConnectionStringBuilder NewPostgreSqlUnixSocketConnectionString()
        {
            // [START cloud_sql_postgres_dotnet_ado_connection_socket]
            // Equivalent connection string:
            // "Server=<dbSocketDir>/<INSTANCE_CONNECTION_NAME>;Uid=<DB_USER>;Pwd=<DB_PASS>;Database=<DB_NAME>"
            String dbSocketDir = Environment.GetEnvironmentVariable("DB_SOCKET_PATH") ?? "/cloudsql";
            String instanceConnectionName = Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME");
            var connectionString = new NpgsqlConnectionStringBuilder()
            {
                // The Cloud SQL proxy provides encryption between the proxy and instance.
                SslMode = SslMode.Disable,
                // Remember - storing secrets in plain text is potentially unsafe. Consider using
                // something like https://cloud.google.com/secret-manager/docs/overview to help keep
                // secrets secret.
                Host = String.Format("{0}/{1}", dbSocketDir, instanceConnectionName),
                Username = Environment.GetEnvironmentVariable("DB_USER"), // e.g. 'my-db-user
                Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
            };
            connectionString.Pooling = true;
            // Specify additional properties here.
            return connectionString;
            // [END cloud_sql_postgres_dotnet_ado_connection_socket]
        }
    }
}
