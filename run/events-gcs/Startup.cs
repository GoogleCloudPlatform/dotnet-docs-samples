// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventsGcs
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // [START run_events_gcs_handler]
                endpoints.MapPost("/", async context =>
                {
                    var ceSubject = context.Request.Headers["ce-subject"];

                    if (string.IsNullOrEmpty(ceSubject))
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Bad Request: expected header Ce-Subject");
                        return;
                    }

                    await context.Response.WriteAsync($"GCS CloudEvent type: {ceSubject}");
                });
                // [END run_events_gcs_handler]
            });
        }
    }
}
