// Copyright 2021 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START pubsub_subscribe_proto_messages]

using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using System;
using System.Threading;
using System.Threading.Tasks;

public class PullProtoMessagesAsyncSample
{
    public async Task<int> PullProtoMessagesAsync(string projectId, string subscriptionId, bool acknowledge)
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
        int messageCount = 0;
        SubscriberClient subscriber = await new SubscriberClientBuilder
        {
            SubscriptionName = subscriptionName,
            Settings = new SubscriberClient.Settings
            {
                AckExtensionWindow = TimeSpan.FromSeconds(4),
                AckDeadline = TimeSpan.FromSeconds(10),
                FlowControlSettings = new FlowControlSettings(maxOutstandingElementCount: 100, maxOutstandingByteCount: 10240)
            }
        }.BuildAsync();
        // SubscriberClient runs your message handle function on multiple
        // threads to maximize throughput.
        Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
        {
            string encoding = message.Attributes["googclient_schemaencoding"];
            Utilities.State state = null;
            switch (encoding)
            {
                case "BINARY":
                    state = Utilities.State.Parser.ParseFrom(message.Data.ToByteArray());
                    break;
                case "JSON":
                    state = Utilities.State.Parser.ParseJson(message.Data.ToStringUtf8());
                    break;
                default:
                    Console.WriteLine($"Encoding not provided in message.");
                    break;
            }
            Console.WriteLine($"Message {message.MessageId}: {state}");
            Interlocked.Increment(ref messageCount);
            return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);
        });
        // Run for 5 seconds.
        await Task.Delay(5000);
        await subscriber.StopAsync(CancellationToken.None);
        // Lets make sure that the start task finished successfully after the call to stop.
        await startTask;
        return messageCount;
    }
}
// [END pubsub_subscribe_proto_messages]
