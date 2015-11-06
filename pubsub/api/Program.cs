using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Google.Apis.Pubsub.v1.Data;

// NOTE PubSub API must be enabled

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
      new GetSubscriptionPolicySample().GetSubscriptionPolicy(projectId: ProjectID, subscriptionName: subscriptionName);
    }
    
    public static void SetSubscriptionPolicy(string subscriptionName, string role, string member)
    {
      IList<Binding> bindings = new[]
      {
        new Binding()
        {
          Role = role,
          Members = new[] { member }
        }
      };

      new SetSubscriptionPolicySample().SetSubscriptionPolicy(
        projectId: ProjectID,
        subscriptionName: subscriptionName,
        bindings: bindings
      );
    }

    // TODO rename Test*Permissions
    // Usage: pubsub.subscriptions.consume
    public static void TestSubscriptionPolicy(string subscriptionName, string permission)
    {
      new TestSubscriptionPermissionsSample().TestSubscriptionPermissions(
        projectId:ProjectID,
        subscriptionName: subscriptionName,
        permissions: new[] { permission }
      );
    }
  }
}
