/*
 * Copyright (c) 2018 Google Inc.
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

// [START error_reporting_setup_aspnetcore_using_diagnostics]

using Google.Cloud.Diagnostics.AspNetCore;
// [END error_reporting_setup_aspnetcore_using_diagnostics]
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErrorReporting
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // [START error_reporting_setup_dotnetcore_configure_services]
            services.AddGoogleExceptionLogging(options =>
            {
                options.ProjectId = "YOUR-GOOGLE-PROJECT-ID";
                options.ServiceName = "ShoppingCartService";
                options.Version = "0.01";
            });
            // [END error_reporting_setup_dotnetcore_configure_services]
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            // [START error_reporting_setup_dotnetcore_configure]
            // Use before handling any requests to ensure all unhandled exceptions are reported.
            app.UseGoogleExceptionLogging();
            // [END error_reporting_setup_dotnetcore_configure]

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
