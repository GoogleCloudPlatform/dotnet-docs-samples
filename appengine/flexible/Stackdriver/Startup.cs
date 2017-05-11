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
            // [START configure_services_logging_and_error_reporting]
            services.AddOptions();
            services.Configure<StackdriverOptions>(
                Configuration.GetSection("StackDriver"));
            // [END configure_services_logging_and_error_reporting]

            // [START configure_services_trace]
            // Add trace service.
            services.AddGoogleTrace(Configuration["StackDriver:ProjectId"]);
            // [END configure_services_trace]

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // [START configure_and_use_logging]
            // Configure logging service.
            loggerFactory.AddGoogle(Configuration["StackDriver:ProjectId"]);
            var logger = loggerFactory.CreateLogger("testStackdriverLogging");
            logger.LogInformation("Stackdriver sample started. This is a log message.");
            // [END configure_and_use_logging]

            // [START configure_error_reporting]
            // Configure error reporting service.
            var options = app.ApplicationServices
                .GetService<IOptions<StackdriverOptions>>().Value;
            app.UseGoogleExceptionLogging(options.ProjectId, 
                options.ServiceName, options.Version);
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
        }
    }
}
