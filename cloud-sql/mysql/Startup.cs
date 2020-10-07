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
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Data.Common;
using System.Data;
using Polly;

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

            connection = GetMySqlConnection();
            connection.Open();
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
            return connection;
        }

        void SetDbConfigOptions(MySqlConnectionStringBuilder connectionString)
        {
            // [START cloud_sql_mysql_dotnet_ado_limit]
            // MaximumPoolSize sets maximum number of connections allowed in the pool.            
            connectionString.MaximumPoolSize = 5;
            // MinimumPoolSize sets the minimum number of connections in the pool.
            connectionString.MinimumPoolSize = 0;
            // [END cloud_sql_mysql_dotnet_ado_limit]
            // [START cloud_sql_mysql_dotnet_ado_timeout]
            // ConnectionTimeout sets the time to wait (in seconds) while
            // trying to establish a connection before terminating the attempt.
            connectionString.ConnectionTimeout = 15;
            // [END cloud_sql_mysql_dotnet_ado_timeout]
            // [START cloud_sql_mysql_dotnet_ado_lifetime]
            // ConnectionLifeTime sets the lifetime of a pooled connection
            // (in seconds) that a connection lives before it is destroyed
            // and recreated. Connections that are returned to the pool are
            // destroyed if it's been more than the number of seconds
            // specified by ConnectionLifeTime since the connection was
            // created. The default value is zero (0) which means the
            // connection always returns to pool.
            connectionString.ConnectionLifeTime = 1800; // 30 minutes
            // [END cloud_sql_mysql_dotnet_ado_lifetime]
        }

        DbConnection NewMysqlTCPConnection()
        {
            // [START cloud_sql_mysql_dotnet_ado_connection_tcp]
            // Equivalent connection string: 
            // "Uid=<DB_USER>;Pwd=<DB_PASS>;Host=<DB_HOST>;Database=<DB_NAME>;"
            var connectionString = new MySqlConnectionStringBuilder()
            {
                // The Cloud SQL proxy provides encryption between the proxy and instance. 
                SslMode = MySqlSslMode.None,

                // Remember - storing secrets in plaintext is potentially unsafe. Consider using
                // something like https://cloud.google.com/secret-manager/docs/overview to help keep
                // secrets secret.
                Server = Environment.GetEnvironmentVariable("DB_HOST"),   // e.g. '127.0.0.1'
                // Set Host to 'cloudsql' when deploying to App Engine Flexible environment
                UserID = Environment.GetEnvironmentVariable("DB_USER"),   // e.g. 'my-db-user'
                Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
            };
            connectionString.Pooling = true;
            // Specify additional properties here.
            // [START_EXCLUDE]
            SetDbConfigOptions(connectionString);
            // [END_EXCLUDE]
            DbConnection connection =
                new MySqlConnection(connectionString.ConnectionString);
            // [END cloud_sql_mysql_dotnet_ado_connection_tcp]
            return connection;
        }

        DbConnection NewMysqlUnixSocketConnection()
        {
            // [START cloud_sql_mysql_dotnet_ado_connection_socket]
            // Equivalent connection string: 
            // "Server=<dbSocketDir>/<INSTANCE_CONNECTION_NAME>;Uid=<DB_USER>;Pwd=<DB_PASS>;Database=<DB_NAME>;Protocol=unix"
            String dbSocketDir = Environment.GetEnvironmentVariable("DB_SOCKET_PATH") ?? "/cloudsql";
            String instanceConnectionName = Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME");
            var connectionString = new MySqlConnectionStringBuilder()
            {
                // The Cloud SQL proxy provides encryption between the proxy and instance. 
                SslMode = MySqlSslMode.None,
                // Remember - storing secrets in plaintext is potentially unsafe. Consider using
                // something like https://cloud.google.com/secret-manager/docs/overview to help keep
                // secrets secret.
                Server = String.Format("{0}/{1}", dbSocketDir, instanceConnectionName),
                UserID = Environment.GetEnvironmentVariable("DB_USER"),   // e.g. 'my-db-user
                Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
                ConnectionProtocol = MySqlConnectionProtocol.UnixSocket
            };
            connectionString.Pooling = true;
            // Specify additional properties here.
            // [START_EXCLUDE]
            SetDbConfigOptions(connectionString);
            // [END_EXCLUDE]
            DbConnection connection =
                new MySqlConnection(connectionString.ConnectionString);
            // [END cloud_sql_mysql_dotnet_ado_connection_socket]
            return connection;
        }

        DbConnection GetMySqlConnection()
        {
            // [START cloud_sql_mysql_dotnet_ado_backoff]
            var connection = Policy
                .HandleResult<DbConnection>(conn => conn.State != ConnectionState.Open)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5)
                }, (result, timeSpan, retryCount, context) =>
                {
                    // Log any warnings here.
                })
                .Execute(() =>
                {
                    // Return a new connection.
                    // [START_EXCLUDE]
                    if (Environment.GetEnvironmentVariable("DB_HOST") != null)
                    {
                        return NewMysqlTCPConnection();
                    } else
                    {
                        return NewMysqlUnixSocketConnection();
                    }
                    // [END_EXCLUDE]
                });
            // [END cloud_sql_mysql_dotnet_ado_backoff]
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
