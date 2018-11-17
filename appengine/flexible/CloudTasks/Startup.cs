/*
 * Copyright (c) 2018 Google LLC.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CloudTasks
{
    // [START gae_flex_quickstart]
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug();
            var logger = loggerFactory.CreateLogger("testStackdriverLogging");

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

            var routeBuilder = new RouteBuilder(app);

            routeBuilder.MapPost("log_payload", context =>
            {
                // Log the request payload
                var Reader = new StreamReader(context.Request.Body);
                var Task = Reader.ReadToEnd();

                logger.LogInformation($"Received task with payload: {Task}");
                return context.Response.WriteAsync("Printed task payload: {Task}");
            });

            routeBuilder.MapGet("hello", context =>
            {
                // Basic index to verify app is serving
                return context.Response.WriteAsync("Hello, world!");
            });

            routeBuilder.MapGet("ah/health", context =>
            {
                // Respond to GAE health-checks
                return context.Response.WriteAsync("OK");
            });        

            var routes = routeBuilder.Build();
            app.UseRouter(routes);


        }
    }
    // [END gae_flex_quickstart]
}
