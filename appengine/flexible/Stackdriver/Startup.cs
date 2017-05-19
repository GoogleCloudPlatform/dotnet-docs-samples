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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Stackdriver
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
        public void ConfigureServices(IServiceCollection services)
        {
            // [START configure_services_logging]
            // [START configure_services_error_reporting]
            services.AddOptions();
            services.Configure<StackdriverOptions>(
                Configuration.GetSection("Stackdriver"));
            // [END configure_services_logging]
            services.AddGoogleExceptionLogging(
                Configuration["Stackdriver:ProjectId"],
                Configuration["Stackdriver:ServiceName"], 
                Configuration["Stackdriver:Version"]);
            // [END configure_services_error_reporting]

            // [START configure_services_trace]
            // Add trace service.
            TraceConfiguration traceConfig = TraceConfiguration.Create(bufferOptions: BufferOptions.NoBuffer());
            services.AddGoogleTrace(Configuration["Stackdriver:ProjectId"], traceConfig);
            // [END configure_services_trace]

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // [START configure_and_use_logging]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Configure logging service.
            loggerFactory.AddGoogle(Configuration["Stackdriver:ProjectId"]);
            var logger = loggerFactory.CreateLogger("testStackdriverLogging");

            // Write the log entry.
            logger.LogInformation("Stackdriver sample started. This is a log message.");
            // [END configure_and_use_logging]

            // [START configure_error_reporting]
            // Configure error reporting service.
            app.UseGoogleExceptionLogging();
            // [END configure_error_reporting]

            // [START configure_trace]
            // Configure trace service.
            app.UseGoogleTrace();
            // [END configure_trace]

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            // [START configure_and_use_logging]
        }
        // [END configure_and_use_logging]
    }
}
