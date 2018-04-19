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
    public class EntityManagementTests : DialogflowTest
    {
        string EntityTypeId { get; set; }
        readonly string EntityValue = TestUtil.RandomName();
        readonly string[] Synonyms = new[] { "synonym1", "synonym2" };
        string SynonymsInput => string.Join(',', Synonyms);

        public EntityManagementTests()
        {
            // Create EntityType for each test.
            // Required as prerequisite for Entity management.
            EntityTypeId = CreateEntityType();
        }

        [Fact]
        void TestCreate()
        {
            Run("entities:list", EntityTypeId);
            Assert.DoesNotContain(EntityValue, Stdout);

            Run("entities:create", EntityTypeId, EntityValue, SynonymsInput);
            Assert.Contains("Waiting for the entity creation operation to complete.", Stdout);
            Assert.Contains("Entity creation completed.", Stdout);

            Run("entities:list", EntityTypeId);
            Assert.Contains(EntityValue, Stdout);
        }

        [Fact]
        void TestDelete()
        {
            Run("entities:create", EntityTypeId, EntityValue, SynonymsInput);
            Run("entities:list", EntityTypeId);
            Assert.Contains(EntityValue, Stdout);

            Run("entities:delete", EntityTypeId, EntityValue);
            Assert.Contains("Waiting for the entity deletion operation to complete.", Stdout);
            Assert.Contains($"Deleted Entity: {EntityValue}", Stdout);

            Run("entities:list", EntityTypeId);
            Assert.DoesNotContain(EntityValue, Stdout);
        }
    }
}
