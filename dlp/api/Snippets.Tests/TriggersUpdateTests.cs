// Copyright 2023 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Cloud.Dlp.V2;
using System.IO;
using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class TriggersUpdateTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;

        public TriggersUpdateTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestTriggerUpdate()
        {
            // Create a trigger first.
            var infoTypes = new InfoType[] { new InfoType { Name = "PERSON_NAME" } };
            var triggerId = $"my-csharp-test-trigger-{Guid.NewGuid()}";
            var fullTriggerId = $"projects/{_fixture.ProjectId}/jobTriggers/{triggerId}";
            var displayName = $"My trigger display name {Guid.NewGuid()}";
            var description = $"My trigger description {Guid.NewGuid()}";

            using var randomBucketFixture = new RandomBucketFixture();
            using var bucketCollector = new BucketCollector(randomBucketFixture.BucketName);
            var bucketName = randomBucketFixture.BucketName;
            var fileName = Guid.NewGuid().ToString();
            var objectName = $"gs://{bucketName}/{fileName}";
            bucketCollector.CopyToBucket(Path.Combine(_fixture.ResourcePath, "dates-input.csv"), fileName);
            var trigger = TriggersCreate.Create(_fixture.ProjectId, bucketName, Likelihood.Unlikely, 1, true, 1, infoTypes, triggerId, displayName, description);

            var infoTypesNew = new InfoType[] { new InfoType { Name = "EMAIL_ADDRESS" } };
            var result = TriggersUpdate.UpdateJob(_fixture.ProjectId, triggerId, infoTypesNew);
            // Trigger should contain new infoTypes.
            Assert.Equal(infoTypesNew, result.InspectJob.InspectConfig.InfoTypes);
            // Delete the created trigger.
            TriggersDelete.Delete(trigger.Name);
        }
    }
}
