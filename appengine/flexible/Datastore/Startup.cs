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

using Google.Cloud.Datastore.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;

namespace Datastore
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
            string projectId = Configuration["GoogleProjectId"];
            if (new string[] { "your-project-id", "", null }.Contains(projectId))
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync(@"<html>
                        <head><title>Error</title></head>
                        <body><p>Set GoogleProjectId to your project id in appsettings.json.
                              <p>See the README.md in the project directory for more information.</p>
                        </body>
                        </html>");
                });
                return;
            }

            DatastoreDb datastore = DatastoreDb.Create(projectId);
            var visitKeyFactory = datastore.CreateKeyFactory("visit");
            app.Run(async (HttpContext context) =>
            {
                // Insert a visit into Datastore:
                Entity newVisit = new Entity();
                newVisit.Key = visitKeyFactory.CreateIncompleteKey();
                newVisit["time_stamp"] = DateTime.UtcNow;
                newVisit["ip_address"] = FormatAddress(
                    context.Connection.RemoteIpAddress);
                await datastore.InsertAsync(newVisit);

                // Look up the last 10 visits.
                var results = await datastore.RunQueryAsync(new Query("visit")
                {
                    Order = { { "time_stamp", PropertyOrder.Types.Direction.Descending } },
                    Limit = 10
                });
                await context.Response.WriteAsync(@"<html>
                    <head><title>Visitor Log</title></head>
                    <body>Last 10 visits:<br>");
                foreach (Entity visit in results.Entities)
                {
                    await context.Response.WriteAsync(string.Format("{0} {1}<br>",
                        visit["time_stamp"].TimestampValue,
                        visit["ip_address"].StringValue));
                }
                await context.Response.WriteAsync(@"</body></html>");
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
