// Copyright 2016 Google Inc.
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

// [START pubsub_quickstart]
using System;
// Imports the Google Cloud client library
using Google.Pubsub.V1;

public class QuickstartSample
{
    public static void Main()
    {
        // Instantiates a client
        PublisherClient publisher = PublisherClient.Create();

        // Your Google Cloud Platform project ID
        string projectId = "YOUR_PROJECT_ID";

        // The name for the new topic
        string projectTopic = "my-new-topic";

        // The fully qualified name for the new topic
        var topicName = new TopicName(projectId, projectTopic);

        // Creates the new topic
        Topic topic = publisher.CreateTopic(topicName);

        Console.WriteLine($"Topic {topic.Name} created.");
    }
}
// [END pubsub_quickstart]
