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

using Xunit;
using Microsoft.AspNet.TestHost;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

public class PubSubPushMessageTest
{
  [Fact]
  public void PushMessageWebHandlerTest()
  {
    var pushRequestBody = new StringContent("{" + 
      @"""subscription"":""SubscriptionName""," +
      @"""message"":{" +
        @"""message_id"":""1234""," +
        @"""data"":""SGVsbG8gd29ybGQ=""," + // Base64 encoded "Hello world"
        @"""attributes"":{" +
          @"""attr1"":""value1""," +
          @"""attr2"":""value2""" +
        "}" +
      "}" +
    "}");
    pushRequestBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    var server = new TestServer(TestServer.CreateBuilder().UseStartup("web"));
    var client = server.CreateClient();
    var response = client.PostAsync("/", pushRequestBody).Result;

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var expectedResponse = "Received Message Id: 1234\n" +
                           "Text: Hello world\n" +
                           "From Subscription: SubscriptionName\n" +
                           "Attribute attr1 = value1\n" +
                           "Attribute attr2 = value2\n";

    Assert.Equal(expectedResponse, response.Content.ReadAsStringAsync().Result);
  }
}