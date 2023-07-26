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
            Random random = new Random();
            // Create the bucket for creating stored infoType.
            var storage = StorageClient.Create();
            var bucket = storage.CreateBucket(_fixture.ProjectId, $"dlp_update_dotnet_test_bucket_samples_{random.Next()}");

            // Create output bucket.
            var bucketOutput = storage.CreateBucket(_fixture.ProjectId, $"dlp_update_dotnet_test_stored_infotype_bucket_{random.Next()}");

            // Upload the file to cloud storage.
            var filePath = Path.Combine(_fixture.ResourcePath, "test.txt");
            using var fileStream = File.OpenRead(filePath);
            storage.UploadObject(bucket.Name, "test.txt", null, fileStream);

            try
            {
                // Create Stored InfoType first.
                var storedInfoTypeId = $"github-usernames-update-{random.Next()}";
                var response = CreateStoredInfoTypes.Create(_fixture.ProjectId, $"gs://{bucket.Name}", storedInfoTypeId);

                var result = UpdateStoredInfoTypes.Update($"gs://{bucket.Name}/test.txt", response.Name, $"gs://{bucketOutput.Name}");

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
            finally
            {
                // Delete the created buckets.
                storage.DeleteBucket(bucketOutput.Name);
                storage.DeleteBucket(bucket.Name, new DeleteBucketOptions { DeleteObjects = true });
            }
        }
    }
}
