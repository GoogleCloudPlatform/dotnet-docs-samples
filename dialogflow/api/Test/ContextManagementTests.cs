// Copyright(c) 2018 Google Inc.
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

using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class ContextManagementTests : DialogflowTest
    {
        readonly string _contextId = TestUtil.RandomName();
        readonly int _lifespanCount = 1;

        [Fact]
        void TestCreate()
        {
            RunWithSessionId("contexts:list");
            Assert.DoesNotContain(_contextId, Stdout);

            RunWithSessionId("contexts:create", _contextId, _lifespanCount);
            var contextPath = $"projects/{ProjectId}/agent/sessions/{SessionId}/contexts/{_contextId}";
            Assert.Contains($"Created Context: {contextPath}", Stdout);

            CleanupAfterTest(() => RunWithSessionId("contexts:delete", 
                _contextId));

            _retryRobot.Eventually(() =>
            {
                RunWithSessionId("contexts:list");
                Assert.Contains(_contextId, Stdout);
            });
        }

        [Fact]
        void TestDelete()
        {
            RunWithSessionId("contexts:create", _contextId);

            _retryRobot.Eventually(() =>
            {
                RunWithSessionId("contexts:list");
                Assert.Contains(_contextId, Stdout);
            });

            RunWithSessionId("contexts:delete", _contextId);
            Assert.Contains($"Deleted Context: {_contextId}", Stdout);

            _retryRobot.Eventually(() =>
            {
                RunWithSessionId("contexts:list");
                Assert.DoesNotContain(_contextId, Stdout);
            });
        }
    }
}
