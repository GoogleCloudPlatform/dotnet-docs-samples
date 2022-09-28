// Copyright 2021 Google LLC
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

// [START pubsub_use_emulator]

using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmulatorSupportSample
{
    public async Task WithEmulatorAsync(string projectId, string topicId, string subscriptionId)
    {
        // Use EmulatorDetection.EmulatorOrProduction to create service clients that will
        // that will connect to the PubSub emulator if the PUBSUB_EMULATOR_HOST environment
        // variable is set, but will otherwise connect to the production environment.

        // Create the PublisherServiceApiClient using the PublisherServiceApiClientBuilder
        // and setting the EmulatorDection property.
        PublisherServiceApiClient publisherService = await new PublisherServiceApiClientBuilder
        {
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction
        }.BuildAsync();

        // Use the client as you'd normally do, to create a topic in this example.
        TopicName topicName = new TopicName(projectId, topicId);
        publisherService.CreateTopic(topicName);

        // Create the SubscriberServiceApiClient using the SubscriberServiceApiClientBuilder
        // and setting the EmulatorDection property.
        SubscriberServiceApiClient subscriberService = await new SubscriberServiceApiClientBuilder
        {
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction
        }.BuildAsync();

        // Use the client as you'd normally do, to create a subscription in this example.
        SubscriptionName subscriptionName = new SubscriptionName(projectId, subscriptionId);
        subscriberService.CreateSubscription(subscriptionName, topicName, pushConfig: null, ackDeadlineSeconds: 60);

        // Create the PublisherClient using PublisherClientBuilder to set the EmulatorDetection property.
        PublisherClient publisher = await new PublisherClientBuilder
        {
            TopicName = topicName,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction
        }.BuildAsync();
        // Use the client as you'd normally do, to send a message in this example.
        await publisher.PublishAsync("Hello, Pubsub");
        await publisher.ShutdownAsync(TimeSpan.FromSeconds(15));

        // Create the SubscriberClient using SubscriberClientBuild to set the EmulatorDetection property.
        SubscriberClient subscriber = await new SubscriberClientBuilder
        {
            SubscriptionName = subscriptionName,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction
        }.BuildAsync();
        List<PubsubMessage> receivedMessages = new List<PubsubMessage>();

        // Use the client as you'd normally do, to listen for messages in this example.
        await subscriber.StartAsync((msg, cancellationToken) =>
        {
            receivedMessages.Add(msg);
            Console.WriteLine($"Received message {msg.MessageId} published at {msg.PublishTime.ToDateTime()}");
            Console.WriteLine($"Text: '{msg.Data.ToStringUtf8()}'");
            // In this example we stop the subscriber when the message is received.
            // You may leave the subscriber running, and it will continue to received published messages
            // if any.
            // This is non-blocking, and the returned Task may be awaited.
            subscriber.StopAsync(TimeSpan.FromSeconds(15));
            // Return Reply.Ack to indicate this message has been handled.
            return Task.FromResult(SubscriberClient.Reply.Ack);
        });
    }
}

// [END pubsub_use_emulator]
