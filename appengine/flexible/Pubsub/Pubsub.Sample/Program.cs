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
            var topic = CreateTopicAndSubscription(options);
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

    static TopicName CreateTopicAndSubscription(PubsubOptions options)
    {
        var topicName = new TopicName(options.ProjectId, options.TopicId);
        var publisher = PublisherServiceApiClient.Create();
        try
        {
            publisher.CreateTopic(topicName);
        }
        catch (Grpc.Core.RpcException e)
        when (e.Status.StatusCode == Grpc.Core.StatusCode.AlreadyExists ||
            e.Status.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
        {
            Console.WriteLine("Could not create topic {0}", topicName);
        }
        // Create push subscription for /Push endpoint.
        var subscriptionName = new SubscriptionName(
                options.ProjectId, options.SubscriptionId);
        var pushConfig = new PushConfig()
        {
            PushEndpoint = $"https://{options.ProjectId}.appspot.com/Push"
        };
        CreateOrModifySubscription(subscriptionName, topicName, pushConfig);

        // Create push subscription for /AuthPush endpoint.
        var authSubscriptionName = new SubscriptionName(
            options.ProjectId, options.AuthSubscriptionId);
        // To enable authentication in subscription.
        var oidcToken = new PushConfig.Types.OidcToken();
        oidcToken.ServiceAccountEmail = options.ServiceAccountEmail;
        var authPushConfig = new PushConfig
        {
            PushEndpoint = $"https://{options.ProjectId}.appspot.com/AuthPush",
            OidcToken = oidcToken
        };
        CreateOrModifySubscription(authSubscriptionName, topicName, authPushConfig);
        return topicName;
    }

    static void CreateOrModifySubscription(SubscriptionName subscriptionName,TopicName topicName,PushConfig pushConfig)
    {
        var subscriber = SubscriberServiceApiClient.Create();
        try
        {
            subscriber.CreateSubscription(subscriptionName, topicName,
                pushConfig, 20);
            return;
        }
        catch (Grpc.Core.RpcException e)
        when (e.Status.StatusCode == Grpc.Core.StatusCode.AlreadyExists ||
            e.Status.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
        {
            Console.WriteLine("Could not create subscription {0}", subscriptionName);
        }
        catch (Grpc.Core.RpcException e)
        {
            Console.WriteLine("Could not create subscription {0}", subscriptionName);
            return;
        }
        try
        {
            subscriber.ModifyPushConfig(subscriptionName, pushConfig);
        }
        catch (Grpc.Core.RpcException e)
        when (e.Status.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
        {
            Console.WriteLine("Could not modify subscription {0}", subscriptionName);
        }
        return;
    }

    public static void Main(string[] args)
    {
        new Program(args).App.Run();
    }
}

