using System;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class CreateSubscriptionSample
{
  public void CreateSubscription(string projectId, string topicName, string subscriptionName)
  {
    PubsubService PubSub = PubSubClient.Create();

      Subscription subscription = PubSub.Projects.Subscriptions.Create(
        name: $"projects/{projectId}/subscriptions/{subscriptionName}",
        body: new Subscription()
        {
          Name = subscriptionName,
          Topic = $"projects/{projectId}/topics/{topicName}"
        }
      ).Execute();

      Console.WriteLine($"Created: {subscription.Name}");
  }
}