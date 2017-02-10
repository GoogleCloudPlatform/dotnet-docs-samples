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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Analytics
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(HomePage);
        }

        private async Task HomePage(HttpContext context)
        {
            string trackingId = Configuration["GA_TRACKING_ID"];
            if (new string[] { null, "", "your-google-analytics-tracking-id" }.Contains(trackingId))
            {
                await context.Response.WriteAsync(@"
                <html>
                <head><title>Error</title></head>
                <body>
                <p>
                Set the environment variable GA_TRACKING_ID to your Google Analytics tracking id.
                <p>
                </body>
                </html>
                ");
                return;
            }
            // [START track_event]
            HttpClient http = new HttpClient()
            {
                BaseAddress = new Uri("http://www.google-analytics.com/")
            };
            var content = new FormUrlEncodedContent(
                new Dictionary<string, string>() {
                    { "v" , "1" },  // API Version.
                    { "tid" , Configuration["GA_TRACKING_ID"] },  // Tracking ID / Property ID.
                    // Anonymous Client Identifier. Ideally, this should be a UUID that
                    // is associated with particular user, device, or browser instance.
                    { "cid" , "555" },
                    { "t" , "event" },  // Event hit type.
                    { "ec" , "Poker" },  // Event category.
                    { "ea" , "Royal Flush" },  // Event action.
                    { "el" , "Hearts" },  // Event label.
                    { "ev" , "0" },  // Event value, must be an integer
                });
            var post = http.PostAsync("collect", content);
            // [END track_event]
            await context.Response.WriteAsync(string.Format(@"
                <html>
                <head><title>Analytics Sample</title></head>
                <body>
                <p>
                You got a Royal Flush!
                <p>
                Analytics response code: <span id='statusCode'>{0}</span>
                </body>
                </html>
            ", (await post).StatusCode));
        }
    }
}