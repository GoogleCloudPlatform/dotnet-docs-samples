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

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PubSubSamplesTest
{

  // TODO helper method to call command and get output

  PubSubTestHelper pubsub = new PubSubTestHelper();

  string PubSubExe(params string[] args)
  {
    using (var output = new ConsoleOutputReader())
    {
      PubSubSample.Program.Main(args);
      return output.ToString();
    }
  }

  [TestInitialize]
  public void Setup()
  {
    pubsub.DeleteAllSubscriptions();
    pubsub.DeleteAllTopics();
  }

  [TestMethod]
  public void TestUsage()
  {
    StringAssert.Contains(PubSubExe(), "Usage: PubSubSample.exe [command] [args]");
  }

  [TestMethod]
  public void TestCommandNotFound()
  {
    StringAssert.Contains(PubSubExe("InvalidCommand"), "Command not found: InvalidCommand");
  }

  [TestMethod]
  public void TestListTopics_None()
  {
    StringAssert.Contains(PubSubExe("ListTopics"), "");
  }

  [TestMethod]
  public void TestListTopics()
  {
    pubsub.CreateTopic("mytopic");

    StringAssert.Contains(PubSubExe("ListTopics"), $"Found topics: projects/{pubsub.ProjectID}/topics/mytopic");
  }

  [TestMethod]
  public void TestListSubscriptions_None()
  {
    StringAssert.Contains(PubSubExe("ListSubscriptions"), "");
  }

  [TestMethod]
  public void TestListSubscriptions()
  {
    pubsub.CreateTopic("mytopic");
    pubsub.CreateSubscription("mytopic", "mysubscription");

    StringAssert.Contains(PubSubExe("ListSubscriptions"), $"Found subscription: projects/{pubsub.ProjectID}/subscriptions/mysubscription");
  }

  [TestMethod]
  public void TestPublishMessage()
  {
    pubsub.CreateTopic("mytopic");
    pubsub.CreateSubscription("mytopic", "mysubscription");

    PubSubExe("PublishMessage", "mytopic", "Hello there!");

    CollectionAssert.Contains(pubsub.PullMessages("mysubscription"), "Hello there!");
  }
}