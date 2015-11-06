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
    static string ProjectID { get { return Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"); } }
    static string PROJECT_NAME { get { return $"projects/{ProjectID}"; }}

    static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine("Usage: PubSubSample.exe [command] [args]");
        Environment.Exit(0);
      }

      // TODO make explicit (not dynamic)
      var commandName = args.FirstOrDefault();
      var commandArguments = args.Skip(1);
      var commandMethod = typeof(Program).GetMethod(commandName, BindingFlags.Static | BindingFlags.Public);

      if (commandMethod == null)
        Console.WriteLine($"Command not found: {commandName}");
      else
        commandMethod.Invoke(null, commandArguments.ToArray());
    }

    // TODO remove
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

    public static void CreateTopic(string topicName)
    {
      new CreateTopicSample().CreateTopic(ProjectID, topicName);
    }

    public static void ListTopics()
    {
      new ListTopicsSample().ListTopics(ProjectID);
    }

    public static void ListSubscriptions()
    {
      new ListSubscriptionsSample().ListSubscriptions(ProjectID);
    }

    // TODO Add PushConfig
    public static void CreateSubscription(string topicName, string subscriptionName)
    {
      new CreateSubscriptionSample().CreateSubscription(
        projectId: ProjectID,
        topicName: topicName,
        subscriptionName: subscriptionName
      );
    }

    public static void PublishMessage(string topicName, string message)
    {
      new PublishMessageSample().PublishMessage(
        projectId: ProjectID,
        topicName: topicName,
        message: message
      );
    }

    public static void Pull(string subscriptionName)
    {
      new PullMessagesSample().PullMessages(projectId: ProjectID, subscriptionName: subscriptionName);
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
