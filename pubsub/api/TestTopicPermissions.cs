using System;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class TestTopicPermissionsSample
{
  public void TestTopicPermissions(string projectId, string topicName, IList<string> permissions)
  {
    PubsubService PubSub = PubSubClient.Create();

    TestIamPermissionsResponse response = PubSub.Projects.Topics.TestIamPermissions(
      resource: $"projects/{projectId}/topics/{topicName}",
      body: new TestIamPermissionsRequest() { Permissions = permissions }
    ).Execute();

    foreach (var permission in permissions)
    {
      if (response.Permissions.Contains(permission))
      {
        Console.WriteLine($"Caller has permission {permission}");
      }
      else
      {
        Console.WriteLine($"Caller does not have persmission {permission}");
      }
    }
  }
}