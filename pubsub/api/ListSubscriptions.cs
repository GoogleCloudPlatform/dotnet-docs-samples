using System;
using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class ListSubscriptionsSample
{
  public void ListSubscriptions(string projectId)
  {
    PubsubService PubSub = PubSubClient.Create();

    ListSubscriptionsResponse response = PubSub.Projects.Subscriptions.List(
      project: $"projects/{projectId}"
    ).Execute();

    if (response != null)
    {
      IList<Subscription> subscriptions = response.Subscriptions;

      foreach (var subscription in subscriptions)
      {
        Console.WriteLine($"Found subscription: {subscription.Name}");
      }
    }
  }
}