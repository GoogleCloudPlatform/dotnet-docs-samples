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
using System.Text.RegularExpressions;
using Xunit;

namespace GoogleCloudSamples
{
    public class SessionEntityTypeManagementTests : DialogflowTest
    {
        public SessionEntityTypeManagementTests()
        {
            // Create EntityType with display name
            // (Required for Session EntityType management)
            Run("entity-types:create", _entityTypeDisplayName, "Map");
            Assert.Equal(0, ExitCode);
        }

        readonly string _entityTypeDisplayName = TestUtil.RandomName();

        [Fact]
        void TestCreateEntityType()
        {
            var entityValues = new[] { TestUtil.RandomName() };
            var entityValuesArgument = string.Join(',', entityValues);

            RunWithSessionId("session-entity-types:list");
            Assert.Equal(0, ExitCode);
            Assert.DoesNotContain(_entityTypeDisplayName, Stdout);

            // Create Session EntityType
            RunWithSessionId("session-entity-types:create", _entityTypeDisplayName, entityValuesArgument);
            Assert.Equal(0, ExitCode);
            Assert.Contains("Created SessionEntityType:", Stdout);
            Assert.Contains(_entityTypeDisplayName, Stdout);

            RunWithSessionId("session-entity-types:list");
            Assert.Equal(0, ExitCode);
            Assert.Contains(_entityTypeDisplayName, Stdout);
        }

        [Fact]
        void TestDeleteEntityType()
        {
            var entityValues = new[] { TestUtil.RandomName() };
            var entityValuesArgument = string.Join(',', entityValues);

            RunWithSessionId("session-entity-types:create", _entityTypeDisplayName, entityValuesArgument);
            Assert.Equal(0, ExitCode);
            Assert.Contains("Created SessionEntityType:", Stdout);

            RunWithSessionId("session-entity-types:list");
            Assert.Equal(0, ExitCode);
            Assert.Contains(_entityTypeDisplayName, Stdout);

            // Delete Session EntityType
            RunWithSessionId("session-entity-types:delete", _entityTypeDisplayName);
            Assert.Equal(0, ExitCode);
            Assert.Contains("Deleted SessionEntityType", Stdout);

            RunWithSessionId("session-entity-types:list");
            Assert.Equal(0, ExitCode);
            Assert.DoesNotContain(_entityTypeDisplayName, Stdout);
        }
    }
}
