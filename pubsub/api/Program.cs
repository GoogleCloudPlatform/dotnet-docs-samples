using System;
using System.Linq;
using System.Collections.Generic;

namespace PubSubSample
{

  class Program
  {
    static string ProjectID { get { return Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"); } }

    static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine(@"Usage: PubSubSample.exe [command] [args]

       ListTopics
       ListSubscriptions
       CreateTopic            [name]
       CreateSubscription     [topic] [name]
       PublishMessage         [topic] [message]
       Pull                   [subscription]
       GetTopicPolicy         [topic]
       GetSubscriptionPolicy  [subscription]
       SetTopicPolicy         [topic]        [role=member,member...] [role=...]
       SetSubscriptionPolicy  [subscription] [role=member,member...] [role=...]
       TestTopicPolicy        [topic]        [permission] [permission...]
       TestSubscriptionPolicy [subscription] [permission] [permission...]
");
        Environment.Exit(0);
      }

      var commandName = args.FirstOrDefault();
      var commandArguments = args.Skip(1).ToArray();

      switch (commandName)
      {
        case "ListTopics":
          ListTopics();
          break;

        case "ListSubscriptions":
          ListSubscriptions();
          break;

        case "CreateTopic":
          CreateTopic(topicName: commandArguments[0]);
          break;

        case "CreateSubscription":
          CreateSubscription(topicName: commandArguments[0], subscriptionName: commandArguments[1]);
          break;

        case "PublishMessage":
          PublishMessage(topicName: commandArguments[0], message: commandArguments[1]);
          break;

        case "Pull":
          Pull(subscriptionName: commandArguments[0]);
          break;

        case "GetTopicPolicy":
          GetTopicPolicy(topicName: commandArguments[0]);
          break;

        case "GetSubscriptionPolicy":
          GetSubscriptionPolicy(subscriptionName: commandArguments[0]);
          break;

        case "SetTopicPolicy":
          var topicName = commandArguments[0];
          var topicPolicyArguments = commandArguments.Skip(1);

          var topicRolesAndMembers = new Dictionary<string, string[]>();

          foreach (var arg in topicPolicyArguments)
          {
            var roleName = arg.Split('"')[0];
            var memberNames = arg.Split('"')[1].Split(',');
            topicRolesAndMembers[roleName] = memberNames;
          }

          SetTopicPolicy(topicName: topicName, rolesAndMembers: topicRolesAndMembers);
          break;

        case "SetSubscriptionPolicy":
          var subscriptionName = commandArguments[0];
          var subscriptionPolicyArguments = commandArguments.Skip(1);

          var subscriptionRolesAndMembers = new Dictionary<string, string[]>();

          foreach (var arg in subscriptionPolicyArguments)
          {
            var roleName = arg.Split('"')[0];
            var memberNames = arg.Split('"')[1].Split(',');
            subscriptionRolesAndMembers[roleName] = memberNames;
          }

          SetSubscriptionPolicy(subscriptionName: subscriptionName, rolesAndMembers: subscriptionRolesAndMembers);
          break;

        case "TestTopicPolicy":
          TestTopicPermissions(topicName: commandArguments[0], permissions: commandArguments.Skip(1).ToList());
          break;

        case "TestSubscriptionPolicy":
          TestSubscriptionPermissions(subscriptionName: commandArguments[0], permissions: commandArguments.Skip(1).ToList());
          break;

        default:
          Console.WriteLine($"Command not found: {commandName}");
          break;
      }
    }

    // TODO remove methods and use Main() to call all samples directly
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
      new PullSample().Pull(projectId: ProjectID, subscriptionName: subscriptionName);
    }

    public static void GetTopicPolicy(string topicName)
    {
      new GetTopicPolicySample().GetTopicPolicy(projectId: ProjectID, topicName: topicName);
    }

    public static void GetSubscriptionPolicy(string subscriptionName)
    {
      new GetSubscriptionPolicySample().GetSubscriptionPolicy(projectId: ProjectID, subscriptionName: subscriptionName);
    }
    
    public static void SetTopicPolicy(string topicName, IDictionary<string, string[]> rolesAndMembers)
    {
      new SetTopicPolicySample().SetTopicPolicy(
        projectId: ProjectID,
        topicName: topicName,
        rolesAndMembers: rolesAndMembers
      );
    }

    public static void SetSubscriptionPolicy(string subscriptionName, IDictionary<string, string[]> rolesAndMembers)
    {
      new SetSubscriptionPolicySample().SetSubscriptionPolicy(
        projectId: ProjectID,
        subscriptionName: subscriptionName,
        rolesAndMembers: rolesAndMembers
      );
    }

    public static void TestTopicPermissions(string topicName, IList<string> permissions)
    {
      new TestTopicPermissionsSample().TestTopicPermissions(
        projectId:ProjectID,
        topicName: topicName,
        permissions: permissions
      );
    }

    // Usage: pubsub.subscriptions.consume
    public static void TestSubscriptionPermissions(string subscriptionName, IList<string> permissions)
    {
      new TestSubscriptionPermissionsSample().TestSubscriptionPermissions(
        projectId:ProjectID,
        subscriptionName: subscriptionName,
        permissions: permissions
      );
    }
  }
}
