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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Metadata
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Run(async (context) =>
            {
                string myGoogleIpAddress = await GetMyGoogleCloudIpAddressAsync();
                string text;
                if (null == myGoogleIpAddress) {
                    // Must not be running on App Engine.
                    text = @"<html><head><title>Metadata Error</title></head>
                    <body>I am not running in the Google Cloud.
                    </body></html>";
                } else {
                    text = string.Format(@"<html><head><title>Metadata Succeeded</title></head>
                        <body>My public IP address is {0}</body></html>", myGoogleIpAddress);
                }
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(text);
            });
        }

        // [START get_my_ip]
        /// <summary>
        /// Query the metadata server to find my ip address.
        /// </summary>
        /// <returns>My ip address, or null if not running on Google Cloud.</returns>
        async Task<string> GetMyGoogleCloudIpAddressAsync()
        {
            var metadataClient = Google.Cloud.Metadata.V1.MetadataClient.Create();
            try
            {
                var result = await metadataClient.GetMetadataAsync(
                    "instance/network-interfaces/0/access-configs/0/external-ip");
                return result.Content.ToString();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                // Must not be running on App Engine.
            }
            return null;
        }
        // [END get_my_ip]
    }
}
