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
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Dnx.Runtime.Infrastructure;
using System.Net;
using Microsoft.AspNet.TestHost;

public class PubSubPushMessageTest
{
  public PubSubPushMessageTest()
  {
    // var environment = CallContextServiceLocator.Locator.ServiceProvider.GetRequiredService.
  }

  [Fact]
  public void PushMessageWebHandlerTest()
  {
    // var server = Microsoft.AspNet.TestHost.TestServer.Create();
    var server = new TestServer(TestServer.CreateBuilder().UseStartup("web"));
    var client = server.CreateClient();

    var response = client.GetAsync("/").Result;
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("<the response text>", response.Content.ReadAsStringAsync().Result);
  }
}