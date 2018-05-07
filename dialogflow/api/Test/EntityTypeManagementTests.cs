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
    public class EntityTypeManagementTests : DialogflowTest
    {
        readonly string _displayName = TestUtil.RandomName();
        readonly string _kindName = "Map";

        [Fact]
        void TestCreate()
        {
            Run("entity-types:list");
            Assert.DoesNotContain(_displayName, Stdout);

            Run("entity-types:create", _displayName, _kindName);
            Assert.Contains("Created EntityType:", Stdout);

            _retryRobot.Eventually(() =>
            {
                Run("entity-types:list");
                Assert.Contains(_displayName, Stdout);
            });
        }

        [Fact]
        void TestDelete()
        {
            // Create a new EntityType via `entity-types:create` and get it's ID
            // The EntityType ID is needed to delete, the display name is not sufficient.
            var entityTypeId = CreateEntityType(_displayName);

            _retryRobot.Eventually(() =>
            {
                Run("entity-types:list");
                Assert.Contains(_displayName, Stdout);
            });

            Run("entity-types:delete", entityTypeId);
            Assert.Contains($"Deleted EntityType: {entityTypeId}", Stdout);

            _retryRobot.Eventually(() =>
            {
                Run("entity-types:list");
                Assert.DoesNotContain(_displayName, Stdout);
            });
        }
    }
}
