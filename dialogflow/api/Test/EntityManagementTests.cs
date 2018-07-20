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
        readonly string _entityValue = TestUtil.RandomName();
        readonly string[] _synonyms = new[] { "synonym1", "synonym2" };
        string SynonymsInput => string.Join(',', _synonyms);

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
            Assert.DoesNotContain(_entityValue, Stdout);

            Run("entities:create", EntityTypeId, _entityValue, SynonymsInput);
            Assert.Contains("Waiting for the entity creation operation to complete.", Stdout);
            Assert.Contains("Entity creation completed.", Stdout);
            CleanupAfterTest("entities:delete", EntityTypeId, _entityValue);

            _retryRobot.Eventually(() =>
            {
                Run("entities:list", EntityTypeId);
                Assert.Contains(_entityValue, Stdout);
            });
        }

        [Fact]
        void TestDelete()
        {
            Run("entities:create", EntityTypeId, _entityValue, SynonymsInput);

            _retryRobot.Eventually(() =>
            {
                Run("entities:list", EntityTypeId);
                Assert.Contains(_entityValue, Stdout);
            });

            Run("entities:delete", EntityTypeId, _entityValue);
            Assert.Contains("Waiting for the entity deletion operation to complete.", Stdout);
            Assert.Contains($"Deleted Entity: {_entityValue}", Stdout);

            _retryRobot.Eventually(() =>
            {
                Run("entities:list", EntityTypeId);
                Assert.DoesNotContain(_entityValue, Stdout);
            });
        }
    }
}
