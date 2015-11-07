using System;
using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class PullSample
{
  // TODO Base64
  public void Pull(string projectId, string subscriptionName)
  {
    PubsubService PubSub = PubSubClient.Create();

    PullResponse response = PubSub.Projects.Subscriptions.Pull(
      subscription: $"projects/{projectId}/subscriptions/{subscriptionName}",
      body: new PullRequest()
      {
        MaxMessages = 10,
        ReturnImmediately = true
      }
    ).Execute();

    if (response.ReceivedMessages != null)
    {
      IList<ReceivedMessage> receivedMessages = response.ReceivedMessages;
      List<string> acknowledgeIds = new List<string>();

      foreach (var receivedMessage in receivedMessages)
      {
        string message = System.Text.Encoding.UTF8.GetString(
          System.Convert.FromBase64String(receivedMessage.Message.Data)
        );

        Console.WriteLine(message);

        acknowledgeIds.Add(receivedMessage.AckId);
      }

      PubSub.Projects.Subscriptions.Acknowledge(
        subscription: $"projects/{projectId}/subscriptions/{subscriptionName}",
        body: new AcknowledgeRequest()
        {
          AckIds = acknowledgeIds
        }
      ).Execute();
    }
    else
    {
      Console.WriteLine("There were no messages");
    }
  }
}