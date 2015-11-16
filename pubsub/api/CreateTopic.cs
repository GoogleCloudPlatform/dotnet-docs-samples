/*
 * Copyright (c) 2015 Google Inc.
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

namespace PubSubSample
{
    // [START create_topic]
    using Google.Apis.Pubsub.v1;
    using Google.Apis.Pubsub.v1.Data;

    public class CreateTopicSample
    {
        public void CreateTopic(string projectId, string topicName)
        {
            PubsubService PubSub = PubSubClient.Create();

            Topic topic = PubSub.Projects.Topics.Create(
              name: $"projects/{projectId}/topics/{topicName}",
              body: new Topic() { Name = topicName }
            ).Execute();

            System.Console.WriteLine($"Created: {topic.Name}");
        }
    }
    // [END create_topic]
}