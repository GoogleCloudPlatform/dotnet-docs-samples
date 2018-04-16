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
    public class EntityTypeManagementTests : DialogflowTest
    {
        [Fact]
        void TestCreateEntityType()
        {
            var entityTypeDisplayName = TestUtil.RandomName();
            var entityTypeKind = "Map";

            Run("entity-types:list");
            Assert.Equal(0, ExitCode);
            Assert.DoesNotContain(entityTypeDisplayName, Stdout);

            Run("entity-types:create", entityTypeDisplayName, entityTypeKind);
            Assert.Equal(0, ExitCode);
            Assert.Contains($"Created EntityType:", Stdout);

            Run("entity-types:list");
            Assert.Equal(0, ExitCode);
            Assert.Contains(entityTypeDisplayName, Stdout);
        }

        [Fact]
        void TestDeleteEntityType()
        {
            var entityTypeDisplayName = TestUtil.RandomName();
            var entityTypeKind = "Map";

            Run("entity-types:create", entityTypeDisplayName, entityTypeKind);
            Assert.Equal(0, ExitCode);

            // Get the ID of the created EntityType from output of entity-types:create.
            // The EntityType ID is needed to delete, the display name is not sufficient.
            var pattern = new Regex("Created EntityType: projects/(?<projectId>[^/]+)/agent/entityTypes/(?<entityTypeId>.*)");
            var match = pattern.Match(Stdout);
            Assert.True(match.Success);
            var entityTypeId = match.Groups["entityTypeId"].Value;

            Run("entity-types:list");
            Assert.Equal(0, ExitCode);
            Assert.Contains(entityTypeDisplayName, Stdout);

            Run("entity-types:delete", entityTypeId);
            Assert.Equal(0, ExitCode);
            Assert.DoesNotContain(entityTypeDisplayName, Stdout);
        }
    }
}
