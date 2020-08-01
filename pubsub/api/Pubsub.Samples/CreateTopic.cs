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

// [START pubsub_create_topic]

using Google.Cloud.PubSub.V1;
using Grpc.Core;
using System;

public class CreateTopicSample
{
    public Topic CreateTopic(string projectId, string topicId)
    {
        PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
        var topicName = TopicName.FromProjectTopic(projectId, topicId);
        Topic topic = null;

        try
        {
            topic = publisher.CreateTopic(topicName);
            Console.WriteLine($"Topic {topic.Name} created.");
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            Console.WriteLine($"Topic {topicName} already exists.");
        }
        return topic;
    }
}
// [END pubsub_create_topic]
