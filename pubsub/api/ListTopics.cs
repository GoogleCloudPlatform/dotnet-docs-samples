using System;
using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class ListTopicsSample
{
  public void ListTopics(string projectId)
  {
    PubsubService PubSub = PubSubClient.Create();

    ListTopicsResponse response = PubSub.Projects.Topics.List(
      project: $"projects/{projectId}"
    ).Execute();

    if (response != null)
    {
      IList<Topic> topics = response.Topics;

      foreach (var topic in topics)
      {
        Console.WriteLine($"Found topics: {topic.Name}");
      }
    }
  }
}