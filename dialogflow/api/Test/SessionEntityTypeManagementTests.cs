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
    public class SessionEntityTypeManagementTests : DialogflowTest
    {
        readonly string _entityTypeDisplayName = TestUtil.RandomName();
        readonly string[] _entityValues = new[] { "value1", "value2" };
        string EntityValuesArgument => string.Join(',', _entityValues);

        public SessionEntityTypeManagementTests()
        {
            // Create EntityType for each test.
            // Required as prerequisite for Session EntityType management.
            CreateEntityType(displayName: _entityTypeDisplayName);
        }

        [Fact]
        void TestCreate()
        {
            RunWithSessionId("session-entity-types:list");
            Assert.DoesNotContain(_entityTypeDisplayName, Stdout);

            RunWithSessionId("session-entity-types:create", _entityTypeDisplayName, EntityValuesArgument);
            Assert.Contains("Created SessionEntityType:", Stdout);
            Assert.Contains(_entityTypeDisplayName, Stdout);

            _retryRobot.Eventually(() =>
           {
               RunWithSessionId("session-entity-types:list");
               Assert.Contains(_entityTypeDisplayName, Stdout);
           });
        }

        [Fact]
        void TestDelete()
        {
            RunWithSessionId("session-entity-types:create", _entityTypeDisplayName, EntityValuesArgument);

            _retryRobot.Eventually(() =>
            {
                RunWithSessionId("session-entity-types:list");
                Assert.Contains(_entityTypeDisplayName, Stdout);
            });

            RunWithSessionId("session-entity-types:delete", _entityTypeDisplayName);
            Assert.Contains($"Deleted SessionEntityType: {_entityTypeDisplayName}", Stdout);

            _retryRobot.Eventually(() =>
            {
                RunWithSessionId("session-entity-types:list");
                Assert.DoesNotContain(_entityTypeDisplayName, Stdout);
            });
        }
    }
}
