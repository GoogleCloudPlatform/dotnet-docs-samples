// Copyright (c) 2018 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Google.Cloud.Datastore.V1;
using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudokumb;
using WebApp.Models;
using WebApp.Services;

namespace WebApp
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
            services.AddOptions();

            // Add exception logging.
            services.AddGoogleExceptionLogging(options =>
            {
                options.ProjectId = Configuration["Google:ProjectId"];
                options.ServiceName = Configuration["Google:AppEngine:ServiceName"] ?? "WebApp";
                options.Version = Configuration["Google:AppEngine:Version"] ?? "0.0";
            });
            // Add trace service.
            services.AddGoogleTrace(options =>
            {
                options.ProjectId = Configuration["Google:ProjectId"];
                options.Options = Google.Cloud.Diagnostics.Common.TraceOptions.Create(
                    bufferOptions: Google.Cloud.Diagnostics.Common.BufferOptions.NoBuffer());
            });

            services.Configure<Models.AccountViewModels.AccountOptions>(
                Configuration.GetSection("Account"));
            services.Configure<PubsubGameBoardQueueOptions>(
                Configuration.GetSection("Google"));
            services.AddSingleton<DatastoreDb>(provider => DatastoreDb.Create(
                Configuration["Google:ProjectId"],
                Configuration["Google:NamespaceId"] ?? ""));
            services.Configure<KmsDataProtectionProviderOptions>(
                Configuration.GetSection("Google"));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultTokenProviders();
            services.AddTransient<IUserStore<ApplicationUser>,
                DatastoreUserStore<ApplicationUser>>();
            services.AddTransient<IUserRoleStore<ApplicationUser>,
                DatastoreUserStore<ApplicationUser>>();
            services.AddTransient<IRoleStore<IdentityRole>,
                DatastoreRoleStore<IdentityRole>>();
            services.AddDatastoreCounter();
            services.AddSingleton<SolveStateStore>();
            services.AddSingleton<IGameBoardQueue, PubsubGameBoardQueue>();
            services.AddAdminSettings();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton<IDataProtectionProvider,
                KmsDataProtectionProvider>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            // Configure logging service.
            loggerFactory.AddGoogle(Configuration["Google:ProjectId"]);

            // Configure trace service.
            app.UseGoogleTrace();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // Configure error reporting service.
                app.UseGoogleExceptionLogging();
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            // Require HTTPS on App Engine.
            var instance = Google.Api.Gax.Platform.Instance();
            if (null != instance.GaeDetails)
            {
                var rewriteOptions = new RewriteOptions()
                {
                    Rules = { new RewriteHttpsOnAppEngine(HttpsPolicy.Required) }
                };
                app.UseRewriter(rewriteOptions);
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