using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Microsoft.AspNetCore.HttpOverrides;

namespace TranslateWorker
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
            services.AddSingleton<FirestoreDb>(provider => 
                FirestoreDb.Create(GetFirestoreProjectId()));
            services.AddSingleton<Google.Cloud.Translation.V2.TranslationClient>(
                Google.Cloud.Translation.V2.TranslationClient.Create());
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

           // So middleware knows requests arrived via https.
            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });
        }

        public static string GetProjectId()
        {
            // Use the service account credentials, if present.
            GoogleCredential googleCredential = Google.Apis.Auth.OAuth2
                .GoogleCredential.GetApplicationDefault();
            if (googleCredential != null)
            {
                ICredential credential = googleCredential.UnderlyingCredential;
                ServiceAccountCredential serviceAccountCredential =
                    credential as ServiceAccountCredential;
                if (serviceAccountCredential != null)
                {
                    return serviceAccountCredential.ProjectId;
                }
            }            
            // Query the metadata server.
            HttpResponseMessage response = new HttpClient().GetAsync(
                @"http://metadata.google.internal/computeMetadata/v1/project/project-id").Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        // Would normally be the same as the regular project id.  But in
        // our test environment, we need a different one.
        public string GetFirestoreProjectId() =>
            Configuration["FIRESTORE_PROJECT_ID"] ?? GetProjectId();
    }
}
