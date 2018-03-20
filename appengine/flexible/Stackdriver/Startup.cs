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
using Google.Cloud.Trace.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Stackdriver
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // [START configure_services_logging]
        // [START configure_services_error_reporting]
        // [START configure_services_trace]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<StackdriverOptions>(
                Configuration.GetSection("Stackdriver"));
            services.AddGoogleExceptionLogging(options =>
            {
                options.ProjectId = Configuration["Stackdriver:ProjectId"];
                options.ServiceName = Configuration["Stackdriver:ServiceName"];
                options.Version = Configuration["Stackdriver:Version"];
            });

            // Add trace service.
            services.AddGoogleTrace(options =>
            {
                options.ProjectId = Configuration["Stackdriver:ProjectId"];
                options.Options = TraceOptions.Create(
                    bufferOptions: BufferOptions.NoBuffer());
            });

            // Add framework services.
            services.AddMvc();
        }
        // [END configure_services_logging]
        // [END configure_services_error_reporting]
        // [END configure_services_trace]

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // [START configure_and_use_logging]
        // [START configure_error_reporting]
        // [START configure_trace]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            // Configure logging service.
            loggerFactory.AddGoogle(Configuration["Stackdriver:ProjectId"]);
            var logger = loggerFactory.CreateLogger("testStackdriverLogging");
            // Write the log entry.
            logger.LogInformation("Stackdriver sample started. This is a log message.");
            // Configure trace service.
            app.UseGoogleTrace();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // Configure error reporting service.
                // MUST be called AFTER UseExceptionHandler().
                app.UseGoogleExceptionLogging();
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        // [END configure_and_use_logging]
        // [END configure_error_reporting]
        // [END configure_trace]
    }
}
