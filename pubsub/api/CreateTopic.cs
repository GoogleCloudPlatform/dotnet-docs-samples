using System;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class CreateTopicSample
{
  public void CreateTopic(string projectId, string topicName)
  {
    PubsubService PubSub = PubSubClient.Create();

    Topic topic = PubSub.Projects.Topics.Create(
      name: $"projects/{projectId}/topics/{topicName}",
      body: new Topic() { Name = topicName }
    ).Execute();

    Console.WriteLine($"Created: {topic.Name}");
  }
}