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

// [START pubsub_quickstart_publisher]
// [START pubsub_publisher_batch_settings]
// [START pubsub_publish]

using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PublishMessagesAsyncSample
{
    public async Task<string> PublishMessagesAsync(PublisherClient publisher, IEnumerable<string> messageTexts)
    {
        // PublisherClient collects messages into appropriately sized
        // batches.
        string result = string.Empty;
        var publishTasks =
            messageTexts.Select(async text =>
            {
                try
                {
                    string message = await publisher.PublishAsync(text);
                    result = $"Published message {message}";
                }
                catch (Exception exception)
                {
                    result = exception.Message;
                }
            });
        await Task.WhenAll(publishTasks);
        return result;
    }
}
// [END pubsub_publish]
// [END pubsub_publisher_batch_settings]
// [END pubsub_quickstart_publisher]
