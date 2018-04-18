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
        readonly string ContextId = TestUtil.RandomName();
        readonly int LifespanCount = 1;

        [Fact]
        void TestCreate()
        {
            RunWithSessionId("contexts:list");
            Assert.DoesNotContain(ContextId, Stdout);

            RunWithSessionId("contexts:create", ContextId, LifespanCount);
            var contextPath = $"projects/{ProjectId}/agent/sessions/{SessionId}/contexts/{ContextId}";
            Assert.Contains($"Created Context: {contextPath}", Stdout);

            RunWithSessionId("contexts:list");
            Assert.Contains(ContextId, Stdout);
        }

        [Fact(Skip = "Not implemented")]
        void TestList()
        {

        }

        [Fact]
        void TestDelete()
        {
            RunWithSessionId("contexts:create", ContextId);
            RunWithSessionId("contexts:list");
            Assert.Contains(ContextId, Stdout);

            RunWithSessionId("contexts:delete", ContextId);
            Assert.Contains($"Deleted Context: {ContextId}", Stdout);

            RunWithSessionId("contexts:list");
            Assert.DoesNotContain(ContextId, Stdout);
        }
    }
}
