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
        readonly string EntityTypeDisplayName = TestUtil.RandomName();
        readonly string[] EntityValues = new[] { "value1", "value2" };
        string EntityValuesArgument => string.Join(',', EntityValues);

        public SessionEntityTypeManagementTests()
        {
<<<<<<< HEAD
            // Create EntityType for each test.
            // Required as prerequisite for Session EntityType management.
            CreateEntityType(EntityTypeDisplayName);
        }

=======
            // Create EntityType with display name
            // (Required for Session EntityType management)
            Run("entity-types:create", _entityTypeDisplayName, "Map");
            Assert.Equal(0, ExitCode);
        }

        readonly string _entityTypeDisplayName = TestUtil.RandomName();

>>>>>>> 6b0ec3bb438e746331215e1a642e1be0da3ccf6d
        [Fact]
        void TestCreate()
        {
            RunWithSessionId("session-entity-types:list");
<<<<<<< HEAD
            Assert.DoesNotContain(EntityTypeDisplayName, Stdout);

            RunWithSessionId("session-entity-types:create", EntityTypeDisplayName, EntityValuesArgument);
=======
            Assert.Equal(0, ExitCode);
            Assert.DoesNotContain(_entityTypeDisplayName, Stdout);

            // Create Session EntityType
            RunWithSessionId("session-entity-types:create", _entityTypeDisplayName, entityValuesArgument);
            Assert.Equal(0, ExitCode);
>>>>>>> 6b0ec3bb438e746331215e1a642e1be0da3ccf6d
            Assert.Contains("Created SessionEntityType:", Stdout);
            Assert.Contains(_entityTypeDisplayName, Stdout);

            RunWithSessionId("session-entity-types:list");
<<<<<<< HEAD
            Assert.Contains(EntityTypeDisplayName, Stdout);
=======
            Assert.Equal(0, ExitCode);
            Assert.Contains(_entityTypeDisplayName, Stdout);
>>>>>>> 6b0ec3bb438e746331215e1a642e1be0da3ccf6d
        }

        [Fact(Skip = "Not implemented")]
        void TestList()
        {

<<<<<<< HEAD
        }
=======
            RunWithSessionId("session-entity-types:create", _entityTypeDisplayName, entityValuesArgument);
            Assert.Equal(0, ExitCode);
            Assert.Contains("Created SessionEntityType:", Stdout);
>>>>>>> 6b0ec3bb438e746331215e1a642e1be0da3ccf6d

        [Fact]
        void TestDelete()
        {
            RunWithSessionId("session-entity-types:create", EntityTypeDisplayName, EntityValuesArgument);
            RunWithSessionId("session-entity-types:list");
<<<<<<< HEAD
            Assert.Contains(EntityTypeDisplayName, Stdout);

            RunWithSessionId("session-entity-types:delete", EntityTypeDisplayName);
            Assert.Contains($"Deleted SessionEntityType: {EntityTypeDisplayName}", Stdout);

            RunWithSessionId("session-entity-types:list");
            Assert.DoesNotContain(EntityTypeDisplayName, Stdout);
=======
            Assert.Equal(0, ExitCode);
            Assert.Contains(_entityTypeDisplayName, Stdout);

            // Delete Session EntityType
            RunWithSessionId("session-entity-types:delete", _entityTypeDisplayName);
            Assert.Equal(0, ExitCode);
            Assert.Contains("Deleted SessionEntityType", Stdout);

            RunWithSessionId("session-entity-types:list");
            Assert.Equal(0, ExitCode);
            Assert.DoesNotContain(_entityTypeDisplayName, Stdout);
>>>>>>> 6b0ec3bb438e746331215e1a642e1be0da3ccf6d
        }
    }
}
