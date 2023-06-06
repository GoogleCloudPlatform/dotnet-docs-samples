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

using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;
using System.Net;

namespace Pubsub;

public class Program
{
    private WebApplication App {get; set;}

    private Program(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var options = new PubsubOptions()
        {
            ProjectId = builder.Configuration["TEST_PROJECT_ID"],
            VerificationToken = builder.Configuration["TEST_VERIFICATION_TOKEN"],
            TopicId = builder.Configuration["TEST_TOPIC_ID"],
            SubscriptionId = builder.Configuration["TEST_SUBSCRIPTION_ID"],
            AuthSubscriptionId = builder.Configuration["TEST_AUTH_SUBSCRIPTION_ID"],
            ServiceAccountEmail = builder.Configuration["TEST_SERVICE_ACCOUNT_EMAIL"]
        };
        try {
            var topic = new TopicName(options.ProjectId, options.TopicId);
            builder.Services.AddSingleton(Options.Create(options));
            builder.Services.AddSingleton(PublisherClient.Create(topic));
            builder.Services.AddMvc();
            App = builder.Build();
            App.UseStaticFiles();
            App.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
        catch {
            // Setup failed -- add a route so we fail gracefully
            App = builder.Build();
            App.MapGet("/", () => "Pubsub setup failed, please ensure the code sample's environment variables are set.");
        }
    }

    public static void Main(string[] args)
    {
        new Program(args).App.Run();
    }
}

