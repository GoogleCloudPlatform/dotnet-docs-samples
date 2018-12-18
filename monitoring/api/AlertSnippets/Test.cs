// Copyright (c) 2018 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Xunit;

public class AlertSnippetsTest : IClassFixture<AlertTestFixture>
{
    private readonly AlertTestFixture _fixture;
    private readonly AlertSnippets _snippets;

    public AlertSnippetsTest(AlertTestFixture fixture)
    {
        _fixture = fixture;
        _snippets = new AlertSnippets();
    }

    [Fact]
    public void TestEnableNotificationChannel()
    {
        var channel = _snippets.EnableNotificationChannel(
            _fixture.Channel.Name);
        Assert.True(channel.Enabled);
    }

    [Fact]
    public void TestDeleteNotificationChannel()
    {
        var channel = _fixture.CreateChannel();
        _snippets.DeleteNotificationChannel(channel.Name);
    }
}