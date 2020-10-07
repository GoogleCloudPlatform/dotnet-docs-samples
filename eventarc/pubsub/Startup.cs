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

// [START eventarc_pubsub_handler]

using CloudNative.CloudEvents;
using Google.Events;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                var cloudEvent = await context.Request.ReadCloudEventAsync();
                logger.LogInformation("Received CloudEvent\n" + GetEventLog(cloudEvent));

                var messagePublishedData = CloudEventConverters.ConvertCloudEventData<MessagePublishedData>(cloudEvent);
                var pubSubMessage = messagePublishedData.Message;
                if (pubSubMessage == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Bad request: Invalid Pub/Sub message format");
                    return;
                }

                var data = pubSubMessage.Data;
                logger.LogInformation($"Data: {data.ToBase64()}");

                var name = data.ToStringUtf8();
                logger.LogInformation($"Extracted name: {name}");

                var id = context.Request.Headers["ce-id"];
                await context.Response.WriteAsync($"Hello {name}! ID: {id}");
            });
        });
    }

    private string GetEventLog(CloudEvent cloudEvent)
    {
        return $"ID: {cloudEvent.Id}\n"
            + $"Source: {cloudEvent.Source}\n"
            + $"Type: {cloudEvent.Type}\n"
            + $"Subject: {cloudEvent.Subject}\n"
            + $"DataSchema: {cloudEvent.DataSchema}\n"
            + $"DataContentType: {cloudEvent.DataContentType}\n"
            + $"Time: {cloudEvent.Time?.ToUniversalTime():yyyy-MM-dd'T'HH:mm:ss.fff'Z'}\n"
            + $"SpecVersion: {cloudEvent.SpecVersion}\n"
            + $"Data: {cloudEvent.Data}";
    }
}
// [END eventarc_pubsub_handler]
