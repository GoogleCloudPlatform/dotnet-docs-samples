// Copyright 2020 Google Inc.
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

// [START pubsub_subscriber_async_pull]
// [START scc_receive_notifications]

using Google.Cloud.PubSub.V1;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class PullMessagesAsyncSample
{
    public async Task<List<string>> PullMessagesAsync(string projectId,
    string subscriptionId, bool acknowledge)
    {
        SubscriptionName subscriptionName = new SubscriptionName(projectId,
            subscriptionId);
        SubscriberClient subscriber = await SubscriberClient.CreateAsync(
            subscriptionName);
        // SubscriberClient runs your message handle function on multiple
        // threads to maximize throughput.
        var result = new List<string>();
        Task startTask = subscriber.StartAsync(
            async (PubsubMessage message, CancellationToken cancel) =>
            {
                string text =
                    Encoding.UTF8.GetString(message.Data.ToArray());
                result.Add($"Message {message.MessageId}: {text}");
                return acknowledge ? SubscriberClient.Reply.Ack
                    : SubscriberClient.Reply.Nack;
            });
        // Run for 5 seconds.
        await Task.Delay(5000);
        await subscriber.StopAsync(CancellationToken.None);
        return result;
    }
}
// [END pubsub_subscriber_async_pull]
// [END scc_receive_notifications]
