﻿/*
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Net;
using System.Data.Common;
using System.Data;

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

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(DbConnection), (IServiceProvider) => {
                // [START connection]
                DbConnection connection = new MySqlConnection(
                    Configuration["CloudSqlConnectionString"]);
                connection.Open();
                // [END connection]
                CreateTable(connection);
                return connection;
            });
            _services = services;
        }

        static void CreateTable(DbConnection connection) {
            using (var createTableCommand = connection.CreateCommand()) {
                createTableCommand.CommandText = @"
                    CREATE TABLE IF NOT EXISTS 
                    visits (
                        time_stamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP, 
                        user_ip CHAR(64)
                    )";
                createTableCommand.ExecuteNonQuery();
            }                           
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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

        public void OldConfigure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            DbConnection connection;

                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync(string.Format(@"<html>
                        <head><title>Error</title></head>
                        <body><p>Set CloudSqlConnectionString to a valid connection string.
                              <p>{0}
                              p>See the README.md in the project directory for more information.</p>
                        </body>
                        </html>", WebUtility.HtmlEncode(e.Message)));
                });
                return;
        

            app.Run(async (HttpContext context) =>
            {
                // [START example]
                // Insert a visit into the database:
                using (var insertVisitCommand = connection.CreateCommand())
                {
                    insertVisitCommand.CommandText =
                        @"INSERT INTO visits (user_ip) values (@user_ip)";
                    var ip = insertVisitCommand.CreateParameter();
                    ip.DbType = DbType.String;
                    ip.Value =
                        FormatAddress(context.Connection.RemoteIpAddress);
                    insertVisitCommand.Parameters["@user_ip"] = ip;
                    await insertVisitCommand.ExecuteNonQueryAsync();
                }

                // Look up the last 10 visits.
                using (var lookupCommand = connection.CreateCommand())
                {
                    lookupCommand.CommandText = @"
                        SELECT * FROM visits
                        ORDER BY time_stamp DESC LIMIT 10";
                    List<string> lines = new List<string>();
                    var reader = await lookupCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                        lines.Add($"{reader.GetString(0)} {reader.GetString(1)}");
                    await context.Response.WriteAsync(string.Format(@"<html>
                        <head><title>Visitor Log</title></head>
                        <body>Last 10 visits:<br>{0}</body>
                        </html>", string.Join("<br>", lines)));
                }
                // [END example]
            });
        }

        private string FormatAddress(IPAddress address)
        {
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                var bytes = address.GetAddressBytes();
                return string.Format("{0:X2}{1:X2}:{2:X2}{3:X2}", bytes[0], bytes[1],
                    bytes[2], bytes[3]);
            }
            else if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                var bytes = address.GetAddressBytes();
                return string.Format("{0}.{1}", bytes[0], bytes[1]);
            }
            else
            {
                return "bad.address";
            }
        }
    }
}
