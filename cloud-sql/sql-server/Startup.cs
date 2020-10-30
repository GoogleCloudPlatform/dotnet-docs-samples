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
using System;
using System.Data.Common;
using System.Data;
using Polly;
using System.Data.SqlClient;

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
            connection = GetSqlServerConnection();
            connection.Open();
            using (var createTableCommand = connection.CreateCommand())
            {
                // Create the 'votes' table if it does not already exist.
                createTableCommand.CommandText = @"
                 IF OBJECT_ID(N'dbo.votes', N'U') IS NULL 
                   BEGIN 
                     CREATE TABLE dbo.votes(
                       vote_id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
                       time_cast datetime NOT NULL,
                       candidate CHAR(6) NOT NULL)
                   END";
                createTableCommand.ExecuteNonQuery();
            }
            return connection;
        }

        DbConnection NewSqlServerConnection()
        {
            // [START cloud_sql_sqlserver_dotnet_ado_connection_tcp]
            // Equivalent connection string: 
            // "User Id=<DB_USER>;Password=<DB_PASS>;Server=<DB_HOST>;Database=<DB_NAME>;"
            var connectionString = new SqlConnectionStringBuilder()
            {
                // Remember - storing secrets in plaintext is potentially unsafe. Consider using
                // something like https://cloud.google.com/secret-manager/docs/overview to help keep
                // secrets secret.
                DataSource = Environment.GetEnvironmentVariable("DB_HOST"),     // e.g. '127.0.0.1' 
                // Set Host to 'cloudsql' when deploying to App Engine Flexible environment
                UserID = Environment.GetEnvironmentVariable("DB_USER"),         // e.g. 'my-db-user'
                Password = Environment.GetEnvironmentVariable("DB_PASS"),       // e.g. 'my-db-password'
                InitialCatalog = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'

                // The Cloud SQL proxy provides encryption between the proxy and instance
                Encrypt = false,
            };
            connectionString.Pooling = true;
            // [START_EXCLUDE]
            // [START cloud_sql_sqlserver_dotnet_ado_limit]
            // MaximumPoolSize sets maximum number of connections allowed in the pool.            
            connectionString.MaxPoolSize = 5;
            // MinimumPoolSize sets the minimum number of connections in the pool.
            connectionString.MinPoolSize = 0;
            // [END cloud_sql_sqlserver_dotnet_ado_limit]
            // [START cloud_sql_sqlserver_dotnet_ado_timeout]
            // ConnectionTimeout sets the time to wait (in seconds) while
            // trying to establish a connection before terminating the attempt.
            connectionString.ConnectTimeout = 15;
            // [END cloud_sql_sqlserver_dotnet_ado_timeout]
            // [START cloud_sql_sqlserver_dotnet_ado_lifetime]
            // ADO.NET connection pooler removes a connection
            // from the pool after it's been idle for approximately
            // 4-8 minutes, or if the pooler detects that the
            // connection with the server no longer exists.
            // [END cloud_sql_sqlserver_dotnet_ado_lifetime]
            // [END_EXCLUDE]
            DbConnection connection =
                new SqlConnection(connectionString.ConnectionString);
            // [END cloud_sql_sqlserver_dotnet_ado_connection_tcp]
            return connection;
        }

        DbConnection GetSqlServerConnection()
        {
            // [START cloud_sql_sqlserver_dotnet_ado_backoff]
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
                .Execute(() => NewSqlServerConnection());
            // [END cloud_sql_sqlserver_dotnet_ado_backoff]
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
