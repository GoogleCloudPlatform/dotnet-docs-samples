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
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        IServiceCollection _services;

        // This method gets called by the runtime. Use this method to add
        // services to the container.
        // For more information on how to configure your application, visit
        // http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(DbConnection), (IServiceProvider) => 
                InitializeDatabase());
            services.AddMvc(options => {
                options.Filters.Add(typeof(DbExceptionFilterAttribute));
            });
            _services = services;
        }

        DbConnection InitializeDatabase() {
            DbConnection connection;
            string database = Configuration["CloudSQL:Database"];
            switch (database.ToLower()) {
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
            using (var createTableCommand = connection.CreateCommand()) {
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

        DbConnection NewMysqlConnection() {
            // [START mysql_connection]
            var connectionString = new MySqlConnectionStringBuilder(
                Configuration["CloudSql:ConnectionString"])
            {
                SslMode = MySqlSslMode.Required,
                CertificateFile = 
                    Configuration["CloudSql:CertificateFile"]
            };
            if (string.IsNullOrEmpty(connectionString.Database))
                connectionString.Database = "visitors";
            DbConnection connection = 
                new MySqlConnection(connectionString.ConnectionString);
            // [END mysql_connection]
            return connection;

        }

        DbConnection NewPostgreSqlConnection() {
            // [START postgresql_connection]
            var connectionString = new NpgsqlConnectionStringBuilder(
                Configuration["CloudSql:ConnectionString"])
            {
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
                UseSslStream = true,
            };
            if (string.IsNullOrEmpty(connectionString.Database))
                connectionString.Database = "visitors";
            NpgsqlConnection connection = 
                new NpgsqlConnection(connectionString.ConnectionString);
            connection.ProvideClientCertificatesCallback +=
                certs => certs.Add(new X509Certificate2(
                    Configuration["CloudSql:CertificateFile"]));
            // [END postgresql_connection]
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
