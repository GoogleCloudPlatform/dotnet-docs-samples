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

// [START pubsub_quickstart_create_topic]

using Google.Cloud.PubSub.V1;
using System;

public class QuickStartSample
{
    public string QuickStart(string projectId)
    {
        // Instantiates a client
        PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();

        // The name for the new topic
        var topicName = TopicName.FromProjectTopic(projectId, "my-new-topic");

        // Creates the new topic
        try
        {
            Topic topic = publisher.CreateTopic(topicName);
            Console.WriteLine($"Topic {topic.Name} created.");
        }
        catch (Grpc.Core.RpcException e)
        when (e.Status.StatusCode == Grpc.Core.StatusCode.AlreadyExists)
        {
            Console.WriteLine($"Topic {topicName} already exists.");
        }
        return topicName.TopicId;
    }
}
// [END pubsub_quickstart_create_topic]
