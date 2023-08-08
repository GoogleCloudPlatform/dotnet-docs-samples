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
// limitations under the License

using Google.Cloud.Dlp.V2;
using Google.Cloud.Storage.V1;
using System;
using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class UpdateStoredInfoTypesTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public UpdateStoredInfoTypesTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestUpdate()
        {
            // Create an input bucket.
            using var randomInputBucketFixture = new RandomBucketFixture();
            var inputBucketName = randomInputBucketFixture.BucketName;
            using var inputBucketCollector = new BucketCollector(inputBucketName);

            // Upload file to bucket.
            var fileName = "test.txt";
            var objectName = $"gs://{inputBucketName}/{fileName}";
            inputBucketCollector.CopyToBucket(Path.Combine(_fixture.ResourcePath, "test.txt"), fileName);

            // Create an output bucket.
            using var randomOutputBucketFixture = new RandomBucketFixture();
            var outputBucketName = randomOutputBucketFixture.BucketName;
            using var outputBucketCollector = new BucketCollector(outputBucketName);

            Random random = new Random();

            // Create Stored InfoType first.
            var storedInfoTypeId = $"github-usernames-update-{random.Next()}";
            var response = CreateStoredInfoTypes.Create(_fixture.ProjectId, $"gs://{inputBucketName}", storedInfoTypeId);

            var result = UpdateStoredInfoTypes.Update($"gs://{inputBucketName}/test.txt", response.Name, $"gs://{outputBucketName}");

            // Check whether stored infoType contains cloud storage file set field or not
            // as we are changing source from Bigquery to GCS.
            Assert.Contains("cloudStorageFileSet", result.ToString());

            var dlp = DlpServiceClient.Create();

            // Delete the created stored infoType.
            dlp.DeleteStoredInfoType(new DeleteStoredInfoTypeRequest
            {
                Name = result.Name
            });
        }
    }
}
