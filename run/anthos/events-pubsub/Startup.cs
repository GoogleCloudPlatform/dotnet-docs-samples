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

// [START run_anthos_events_pubsub_handler]

using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        logger.LogInformation("Service is starting...");

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapPost("/", async context =>
            {
                logger.LogInformation("Handling HTTP POST");

                using (var reader = new StreamReader(context.Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    logger.LogInformation($"HTTP POST body: {body}");

                    dynamic cloudEventData = JsonConvert.DeserializeObject(body);
                    if (cloudEventData == null)
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Bad request: No Pub/Sub message received");
                        return;
                    }

                    dynamic pubSubMessage = cloudEventData["message"];
                    if (pubSubMessage == null)
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Bad request: Invalid Pub/Sub message format");
                        return;
                    }

                    var data = (string)pubSubMessage["data"];
                    var name = Encoding.UTF8.GetString(Convert.FromBase64String(data));
                    logger.LogInformation($"Extracted name: {name}");

                    var id = context.Request.Headers["ce-id"];
                    await context.Response.WriteAsync($"Hello {name}! ID: {id}");
                }
            });
        });
    }
}
// [END run_anthos_events_pubsub_handler]
