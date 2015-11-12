/*
 * Copyright (c) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PubSubSamplesTest
{
  string PubsubServiceAccount { get { return $"serviceAccount:{System.Environment.GetEnvironmentVariable("PUBSUB_SERVICE_ACCOUNT")}"; } }

  PubSubTestHelper pubsub = new PubSubTestHelper();

  string PubSubExe(params string[] args)
  {
    using (var output = new ConsoleOutputReader())
    {
      PubSubSample.Program.Main(args);
      return output.ToString();
    }
  }

  public void Retry(int times, Action assert)
  {
    for (var i = 0; i < times; i++)
    {
      try
      {
        assert();
        return;
      }
      catch (AssertFailedException failure)
      {
        if (i == times - 1)
          throw failure;
      }
    }
  }

  [TestInitialize]
  public void Setup()
  {
    pubsub.DeleteAllSubscriptions();
    pubsub.DeleteAllTopics();
  }

  [ClassCleanup]
  public static void ClassCleanup()
  {
    var pubsub = new PubSubTestHelper();
    pubsub.DeleteAllSubscriptions();
    pubsub.DeleteAllTopics();
  }

  [TestMethod]
  public void UsageTest()
  {
    StringAssert.Contains(PubSubExe(), "Usage: PubSubSample.exe [command] [args]");
  }

  [TestMethod]
  public void CommandNotFoundTest()
  {
    StringAssert.Contains(PubSubExe("InvalidCommand"), "Command not found: InvalidCommand");
  }

  [TestMethod]
  public void NoTopics_ListTopicsTest()
  {
    StringAssert.Contains(PubSubExe("ListTopics"), "");
  }

  [TestMethod]
  public void ListTopicsTest()
  {
    pubsub.CreateTopic("mytopic");

    StringAssert.Contains(PubSubExe("ListTopics"), $"Found topics: projects/{pubsub.ProjectID}/topics/mytopic");
  }

  [TestMethod]
  public void NoSubscriptions_ListSubscriptionsTest()
  {
    StringAssert.Contains(PubSubExe("ListSubscriptions"), "");
  }

  [TestMethod]
  public void ListSubscriptionsTest()
  {
    pubsub.CreateTopic("mytopic");
    pubsub.CreateSubscription("mytopic", "mysubscription");

    StringAssert.Contains(PubSubExe("ListSubscriptions"), $"Found subscription: projects/{pubsub.ProjectID}/subscriptions/mysubscription");
  }

  [TestMethod]
  public void PublishMessageTest()
  {
    pubsub.CreateTopic("mytopic");
    pubsub.CreateSubscription("mytopic", "mysubscription");
    PubSubExe("PublishMessage", "mytopic", "Hello there!");

    Retry(times: 5, assert: () => {
      CollectionAssert.Contains(pubsub.PullMessages("mysubscription"), "Hello there!");
    });
  }

  [TestMethod]
  public void PullMessageTest()
  {
    pubsub.CreateTopic("mytopic");
    pubsub.CreateSubscription("mytopic", "mysubscription");
    pubsub.PublishMessage("mytopic", "Hello there.");

    // Published messages may not show up right away.
    Retry(times: 5, assert: () => {
      StringAssert.Contains(PubSubExe("Pull", "mysubscription"), "Hello there.");
    });
  }

  public void PushMessageTest() { }

  [TestMethod]
  public void NoPolicy_GetTopicPolicyTest()
  {
    pubsub.CreateTopic("mytopic");

    StringAssert.Contains(
      PubSubExe("GetTopicPolicy", "mytopic"),
      "Topic has no policy"
    );
  }

  [TestMethod]
  public void GetTopicPolicyTest()
  {
    pubsub.CreateTopic("mytopic");

    pubsub.SetTopicPolicy("mytopic", new Dictionary<string, string[]>
    {
      ["roles/viewer"] = new[] { PubsubServiceAccount }
    });

    StringAssert.Contains(
      PubSubExe("GetTopicPolicy", "mytopic"),
      $"{PubsubServiceAccount} is member of role roles/viewer"
    );
  }

  [TestMethod]
  public void SetTopicPolicyTest()
  {
    pubsub.CreateTopic("mytopic");

    PubSubExe("SetTopicPolicy", "mytopic", $"roles/viewer={PubsubServiceAccount}");

    var policy = pubsub.GetTopicPolicy("mytopic");
    Assert.AreEqual(1, policy.Bindings.Count);

    var binding = policy.Bindings.First();
    Assert.AreEqual("roles/viewer", binding.Role);
    Assert.AreEqual(1, binding.Members.Count);
    Assert.AreEqual(PubsubServiceAccount, binding.Members[0]);
  }

  [TestMethod]
  public void TestTopicPermissionsTest()
  {
    pubsub.CreateTopic("mytopic");

    StringAssert.Contains(
      PubSubExe("TestTopicPermissions", "mytopic", "pubsub.topics.publish"),
      "Caller has permission pubsub.topics.publish"
    );
  }

  [TestMethod]
  public void NoPolicy_GetSubscriptionPolicyTest()
  {
    pubsub.CreateTopic("mytopic");
    pubsub.CreateSubscription("mytopic", "mysubscription");

    StringAssert.Contains(
      PubSubExe("GetSubscriptionPolicy", "mysubscription"),
      "Subscription has no policy"
    );
  }

  [TestMethod]
  public void GetSubscriptionPolicyTest()
  {
    pubsub.CreateTopic("mytopic");
    pubsub.CreateSubscription("mytopic", "mysubscription");

    pubsub.SetSubscriptionPolicy("mysubscription", new Dictionary<string, string[]>
    {
      ["roles/viewer"] = new[] { PubsubServiceAccount }
    });

    StringAssert.Contains(
      PubSubExe("GetSubscriptionPolicy", "mysubscription"),
      $"{PubsubServiceAccount} is member of role roles/viewer"
    );
  }

  [TestMethod]
  public void SetSubscriptionPolicyTest()
  {
    pubsub.CreateTopic("mytopic");
    pubsub.CreateSubscription("mytopic", "mysubscription");

    PubSubExe("SetSubscriptionPolicy", "mysubscription", $"roles/viewer={PubsubServiceAccount}");

    var policy = pubsub.GetSubscriptionPolicy("mysubscription");
    Assert.AreEqual(1, policy.Bindings.Count);

    var binding = policy.Bindings.First();
    Assert.AreEqual("roles/viewer", binding.Role);
    Assert.AreEqual(1, binding.Members.Count);
    Assert.AreEqual(PubsubServiceAccount, binding.Members[0]);
  }

  [TestMethod]
  public void TestSubscriptionPermissionsTest()
  {
    pubsub.CreateTopic("mytopic");
    pubsub.CreateSubscription("mytopic", "mysubscription");

    StringAssert.Contains(
      PubSubExe("TestSubscriptionPermissions", "mysubscription", "pubsub.subscriptions.consume"),
      "Caller has permission pubsub.subscriptions.consume"
    );
  }
}