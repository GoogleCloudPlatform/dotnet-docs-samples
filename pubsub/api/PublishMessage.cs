using System;
using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

// TODO Base64
class PublishMessageSample
{
  public void PublishMessage(string projectId, string topicName, string message)
  {
    PubsubService PubSub = PubSubClient.Create();

    message = System.Convert.ToBase64String(
      System.Text.Encoding.UTF8.GetBytes(message)  
    );

    PublishResponse response = PubSub.Projects.Topics.Publish(
      topic: $"projects/{projectId}/topics/{topicName}",
      body: new PublishRequest()
      {
        Messages = new[]
        {
          new PubsubMessage() { Data = message }
        }
      }
    ).Execute();

    if (response.MessageIds != null)
    {
      IList<string> messageIds = response.MessageIds;

      foreach (var messageId in messageIds)
      {
        Console.WriteLine($"Published message ID: {messageId}");
      }
    }
  }
}