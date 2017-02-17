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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Net;

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

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            MySqlConnection connection;
            try
            {
                string connectionString = Configuration["CloudSqlConnectionString"];
                // [START example]
                connection = new MySqlConnection(connectionString);
                connection.Open();
                var createTableCommand = new MySqlCommand(@"CREATE TABLE IF NOT EXISTS visits
                (time_stamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP, user_ip CHAR(64))", connection);
                createTableCommand.ExecuteNonQuery();
                // [END example]
            }
            catch (Exception e)
            {
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
            }

            app.Run(async (HttpContext context) =>
            {
                // [START example]
                // Insert a visit into the database:
                using (var insertVisitCommand = new MySqlCommand(
                        @"INSERT INTO visits (user_ip) values (@user_ip)",
                        connection))
                {
                    insertVisitCommand.Parameters.AddWithValue("@user_ip",
                        FormatAddress(context.Connection.RemoteIpAddress));
                    await insertVisitCommand.ExecuteNonQueryAsync();
                }

                // Look up the last 10 visits.
                using (var lookupCommand = new MySqlCommand(
                    @"SELECT * FROM visits ORDER BY time_stamp DESC LIMIT 10",
                    connection))
                {
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