using System;
using System.Linq;
using System.Reflection;

using Google.Apis.Services;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

// NOTE: PubSub API must be enabled

namespace PubSubSample
{

  class Program
  {
    static string PROJECT_NAME {
      get {
        return "projects/" + Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
      }
    }

    static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine("Usage: PubSubSample.exe [command] [args]");
        Environment.Exit(0);
      }

      var commandName = args.FirstOrDefault();
      var commandArguments = args.Skip(1);
      var commandMethod = typeof(Program).GetMethod(commandName, BindingFlags.Static | BindingFlags.Public);

      if (commandMethod == null)
        Console.WriteLine($"Command not found: {commandName}");
      else
        commandMethod.Invoke(null, commandArguments.ToArray());
    }

    static PubsubService PubSub
    {
      get
      {
        var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync().Result;
        credentials = credentials.CreateScoped(new[] { PubsubService.Scope.Pubsub });

        var serviceInitializer = new BaseClientService.Initializer()
        {
          ApplicationName = "PubSub Sample",
          HttpClientInitializer = credentials
        };

        return new PubsubService(serviceInitializer);
      }
    }

    public static void CreateTopic(string name)
    {
      Console.WriteLine($"Creating topic: {name}");

      var topicRequest = new Topic() { Name = name };
      var topicFullName = $"{PROJECT_NAME}/topics/{name}";

      var topic = PubSub.Projects.Topics.Create(topicRequest, topicFullName).Execute();

      Console.WriteLine($"Created topic: {topic.Name}");
    }

    public static void ListTopics()
    {
      Console.WriteLine("Listing topics");

      var topics = PubSub.Projects.Topics.List(PROJECT_NAME).Execute();

      if (topics != null)
        foreach (var topic in topics.Topics)
          Console.WriteLine(topic.Name);
    }

    public static void ListSubscriptions()
    {
      Console.WriteLine("Listing subscriptions");

      var subscriptions = PubSub.Projects.Subscriptions.List(PROJECT_NAME).Execute();

      if (subscriptions.Subscriptions != null)
        foreach (var subscription in subscriptions.Subscriptions)
          Console.WriteLine($"{subscription.Name} for topic {subscription.Topic}");
    }

    // TODO Add PushConfig
    public static void CreateSubscription(string topicName, string name)
    {
      Console.WriteLine($"Creating subscription {name} for topic {topicName}");

      var fullTopicName = $"{PROJECT_NAME}/topics/{topicName}";
      var subscriptionRequest = new Subscription()
      {
        Name = name,
        Topic = fullTopicName
      };
      var fullSubscriptionName = $"{PROJECT_NAME}/subscriptions/{name}";

      var subscription = PubSub.Projects.Subscriptions.Create(subscriptionRequest, fullSubscriptionName).Execute();

      Console.WriteLine($"Created subscription: {subscription.Name} to topic {subscription.Topic}");
    }

    public static void PublishMessage(string topicName, string message)
    {
      var publishRequest = new PublishRequest()
      {
        Messages = new[] { new PubsubMessage() { Data = message } }
      };
      var fullTopicName = $"{PROJECT_NAME}/topics/{topicName}";

      var response = PubSub.Projects.Topics.Publish(publishRequest, fullTopicName).Execute();

      Console.WriteLine($"Published message to {topicName}");
      foreach (var id in response.MessageIds)
        Console.WriteLine($"id: {id}");
    }

    public static void Pull(string subscriptionName)
    {
      Console.WriteLine($"Pulling latest messages for subscription: {subscriptionName}");

      var pullRequest = new PullRequest()
      {
        MaxMessages = 10,
        ReturnImmediately = true
      };
      var fullSubscriptionName = $"{PROJECT_NAME}/subscriptions/{subscriptionName}";

      var response = PubSub.Projects.Subscriptions.Pull(pullRequest, fullSubscriptionName).Execute();

      if (response.ReceivedMessages != null)
        foreach (var message in response.ReceivedMessages)
        {
          Console.WriteLine($"[{message.AckId}] {message.Message.Data}");
          var ackRequest = new AcknowledgeRequest()
          {
            AckIds = new[] { message.AckId } // 1 by 1 for right now
          };
          PubSub.Projects.Subscriptions.Acknowledge(ackRequest, fullSubscriptionName);
        }
    }

    public static void GetSubscriptionPolicy(string subscriptionName)
    {
      Console.WriteLine($"Getting IAM policy for subscription: {subscriptionName}");

      var fullSubscriptionName = $"{PROJECT_NAME}/subscriptions/{subscriptionName}";

      var policy = PubSub.Projects.Subscriptions.GetIamPolicy(fullSubscriptionName).Execute();

      if (policy.Bindings != null)
      {
        foreach (var binding in policy.Bindings)
        {
          Console.WriteLine($"Role: {binding.Role}");
          foreach (var member in binding.Members)
            Console.WriteLine($" - {member}");
        }
      }
    }

    // NOTE This overwrites any existing policy on the subscription
    //
    // Usage: serviceAccount:myproject@appspot.gserviceaccount.com roles/pubsub.subscriber
    public static void SetSubscriptionPolicy(string subscriptionName, string role, string member)
    {
      Console.WriteLine($"Setting IAM policy for subscription: {subscriptionName}");
      Console.WriteLine($"Add member {member} to role {role}");

      var fullSubscriptionName = $"{PROJECT_NAME}/subscriptions/{subscriptionName}";
      var policyRequest = new SetIamPolicyRequest()
      {
        Policy = new Policy()
        {
          Bindings = new[] {
            new Binding() {
              Members = new[] { member },
                      Role = role
            }
          }
        }
      };

      var policy = PubSub.Projects.Subscriptions.SetIamPolicy(policyRequest, fullSubscriptionName).Execute();

      Console.WriteLine("Set policy");
      foreach (var binding in policy.Bindings)
      {
        Console.WriteLine($"Role: {binding.Role}");
        foreach (var theMember in binding.Members)
          Console.WriteLine($" - {theMember}");
      }
    }

    // Usage: pubsub.subscriptions.consume
    public static void TestSubscriptionPolicy(string subscriptionName, string permission)
    {
      Console.WriteLine($"Checking if you have {permission} permission on subscription {subscriptionName}");

      var fullSubscriptionName = $"{PROJECT_NAME}/subscriptions/{subscriptionName}";
      var testPermissionsRequest = new TestIamPermissionsRequest()
      {
        Permissions = new[] { permission }
      };

      var response = PubSub.Projects.Subscriptions.TestIamPermissions(testPermissionsRequest, fullSubscriptionName).Execute();

      Console.WriteLine("Caller has the following permissions (of those requested):");
      foreach (var hasPermission in response.Permissions)
        Console.WriteLine($" - {hasPermission}");
    }
  }
}
