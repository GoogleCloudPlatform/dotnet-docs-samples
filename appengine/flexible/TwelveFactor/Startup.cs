﻿/*
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwelveFactor.Services;

namespace TwelveFactor
{
    public class Startup
    {
        // Polling a file on Cloud Storage at the rate of
        // once every 2 minutes will cost approximately $0.01 per month.
        // See https://cloud.google.com/storage/pricing#network-pricing
        CloudStorageFileProvider _cloudStorage = new CloudStorageFileProvider(
            TimeSpan.FromMinutes(2));
        public Startup(IHostingEnvironment env)
        {
            // Have to build the configuration twice.
            // 1. To discover the project id.
            Configuration = BuildConfiguration(env, null);
            // 2. To load from the project's Google Cloud Storage bucket.
            string cloudStorageBucket = GetProjectId() + ".appspot.com";
            Configuration = BuildConfiguration(env, cloudStorageBucket);
        }

        /// <summary> Builds a configuration. </summary>
        /// <param name="cloudStorageBucket">
        /// If null, no appsettings.json are loaded from Google Cloud Storage.
        /// </param>
        IConfigurationRoot BuildConfiguration(IHostingEnvironment env, 
            string cloudStorageBucket)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true,
                     reloadOnChange: true);
            if (null != cloudStorageBucket) {
                builder.AddJsonFile(_cloudStorage, string.Format(
                    "{0}/aspnet-configs/appsettings.json", cloudStorageBucket),
                    optional: true, reloadOnChange:true);
            }
            builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", 
                optional: true);
            if (null != cloudStorageBucket) {
                builder.AddJsonFile(_cloudStorage, string.Format(
                    "{0}/aspnet-configs/appsettings.{1}.json",
                    cloudStorageBucket, env.EnvironmentName),
                    optional: true, reloadOnChange:true);
            }
            builder.AddEnvironmentVariables();
            return builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var projectId = GetProjectId();
            // Add framework services.Microsoft.VisualStudio.ExtensionManager.ExtensionManagerService
            services.AddMvc();
            // Enables Stackdriver Trace.
            services.AddGoogleTrace(projectId);
            // Sends Exceptions to Stackdriver Error Reporting.
            services.AddGoogleExceptionLogging(projectId, GetServiceName(), GetVersion());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var projectId = GetProjectId();

            // Only use Console and Debug logging during development.
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            // Send logs to Stackdriver Logging.
            loggerFactory.AddGoogle(projectId);
            
            _cloudStorage.Logger = loggerFactory.CreateLogger<CloudStorageFileProvider>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseGoogleExceptionLogging();
            }

            app.UseStaticFiles();
            app.UseGoogleTrace();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private string GetProjectId()
        {
            var instance = Google.Api.Gax.Platform.Instance();
            var projectId = instance?.ProjectId ?? Configuration["Google:ProjectId"];
            if (string.IsNullOrEmpty(projectId))
            {
                throw new Exception(
                    "The logging, tracing and error reporting libraries need a project ID. " +
                    "Update appsettings.json by setting the ProjectId property with your " +
                    "Google Cloud Project ID, then recompile.");
            }
            return projectId;
        }

        private string GetServiceName()
        {
            var instance = Google.Api.Gax.Platform.Instance();
            // An identifier of the service. See https://cloud.google.com/error-reporting/docs/formatting-error-messages#FIELDS.service.
            var serviceName =
                instance?.GaeDetails?.ServiceId ??
                Configuration["Google:ErrorReporting:ServiceName"];
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new Exception(
                    "The error reporting library needs a service name. " +
                    "Update appsettings.json by setting the Google:ErrorReporting:ServiceName property with your " +
                    "Service Id, then recompile.");
            }
            return serviceName;
        }

        private string GetVersion()
        {
            var instance = Google.Api.Gax.Platform.Instance();
            // The source version of the service. See https://cloud.google.com/error-reporting/docs/formatting-error-messages#FIELDS.version.
            var versionId =
                instance?.GaeDetails?.VersionId ??
                Configuration["Google:ErrorReporting:Version"];
            if (string.IsNullOrEmpty(versionId))
            {
                throw new Exception(
                    "The error reporting library needs a version id. " +
                    "Update appsettings.json by setting the Google:ErrorReporting:Version property with your " +
                    "service version id, then recompile.");
            }
            return versionId;
        }
    }
}
