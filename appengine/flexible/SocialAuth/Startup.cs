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
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SocialAuth.Data;
using SocialAuth.Models;
using SocialAuth.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.DataProtection;
using Google.Cloud.Diagnostics.AspNetCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SocialAuth
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true,
                    reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                    optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see 
                // http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }
            builder.Add(new MetadataConfigurationSource());
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddOptions();
            services.Configure<KmsDataProtectionProviderOptions>(
                          Configuration.GetSection("KmsDataProtection"));
            string connectionString = Configuration
                .GetConnectionString("DefaultConnection");
            services.AddDistributedSqlServerCache((options) =>
            {
                options.ConnectionString = connectionString;
                options.TableName = "Sessions";
            });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddSingleton<IDataProtectionProvider,
                KmsDataProtectionProvider>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // Add exception logging so I can debug issues in production.
            string projectId = GetProjectId();
            services.AddGoogleTrace(projectId);
            services.AddGoogleExceptionLogging(projectId,
                Configuration["GoogleErrorReporting:ServiceName"],
                    Configuration["GoogleErrorReporting:Version"]);
        }

        string GetProjectId() => Configuration["KmsDataProtection:ProjectId"];

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddGoogle(GetProjectId());
            loggerFactory.AddDebug();

            // Configure redirects to HTTPS.
            var rewriteOptions = new RewriteOptions();
            if (Configuration["IAmRunningInGoogleCloud"] == "true")
            {
                rewriteOptions.Add(new RewriteHttpsOnAppEngine(
                    HttpsPolicy.Required));
            }
            else
            {
                rewriteOptions.AddRedirectToHttps(302, 44393);
            }
            app.UseRewriter(rewriteOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseGoogleExceptionLogging();
                app.UseGoogleTrace();
            }

            app.UseStaticFiles();

            app.UseIdentity();

            int authenticationProviderCount = 0;
            // Add external authentication middleware below. To configure them 
            // please see http://go.microsoft.com/fwlink/?LinkID=532715
            string googleClientId =
                Configuration["Authentication:Google:ClientId"];
            if (!string.IsNullOrWhiteSpace(googleClientId))
            {
                app.UseGoogleAuthentication(new GoogleOptions()
                {
                    ClientId = googleClientId,
                    ClientSecret = Configuration[
                        "Authentication:Google:ClientSecret"],
                });
                authenticationProviderCount += 1;
            }

            string facebookAppId =
                Configuration["Authentication:Facebook:AppId"];
            if (!string.IsNullOrWhiteSpace(facebookAppId))
            {
                app.UseFacebookAuthentication(new FacebookOptions()
                {
                    AppId = facebookAppId,
                    AppSecret = Configuration[
                        "Authentication:Facebook:AppSecret"],
                });
                authenticationProviderCount += 1;
            }

            if (0 == authenticationProviderCount)
            {
                app.Run(RequireAuthenticationProviderHandler);
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// When the programmer hasn't configured an authentication provider,
        /// provide a clear error message.
        /// </summary>
        Task RequireAuthenticationProviderHandler(HttpContext context)
        {
            var result = new ContentResult()
            {
                Content = "You must configure an authentication "
                + "provider."
                + "\nSee README.md in the project source directory.",
                ContentType = "text/plain",
                StatusCode = 500
            };
            return result.ExecuteResultAsync(new ActionContext()
            {
                HttpContext = context
            });
        }
    }
}
