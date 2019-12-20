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

// [START trace_setup_aspnetcore_using_diagnostics]

using Google.Cloud.Diagnostics.AspNetCore;
using Google.Cloud.Diagnostics.Common;
// [END trace_setup_aspnetcore_using_diagnostics]
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
        // [START gae_flex_use_logging]
        // [START configure_services_error_reporting]
        // [START trace_setup_aspnetcore_configure_services]
        // [START error_reporting_setup_dotnetcore_configure_services]
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
        // [END error_reporting_setup_dotnetcore_configure_services]
        // [END gae_flex_use_logging]
        // [END configure_services_error_reporting]
        // [END trace_setup_aspnetcore_configure_services]

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // [START gae_flex_configure_logging]
        // [START configure_error_reporting]
        // [START trace_setup_aspnetcore_configure_trace]
        // [START error_reporting_setup_dotnetcore_configure]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Configure logging service.
            loggerFactory.AddGoogle(app.ApplicationServices, Configuration["Stackdriver:ProjectId"]);
            var logger = loggerFactory.CreateLogger("testStackdriverLogging");
            // Write the log entry.
            logger.LogInformation("Stackdriver sample started. This is a log message.");
            // Configure error reporting service.
            app.UseGoogleExceptionLogging();
            // Configure trace service.
            app.UseGoogleTrace();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        // [END error_reporting_setup_dotnetcore_configure]
        // [END gae_flex_configure_logging]
        // [END configure_error_reporting]
        // [END trace_setup_aspnetcore_configure_trace]
    }
}
