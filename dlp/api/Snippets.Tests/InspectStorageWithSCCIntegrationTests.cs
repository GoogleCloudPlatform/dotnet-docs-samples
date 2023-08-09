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

using Google.Cloud.Storage.V1;
using System;
using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectStorageWithSCCIntegrationTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public InspectStorageWithSCCIntegrationTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestCreate()
        {
            Random random = new Random();
            // Create the bucket.
            var storage = StorageClient.Create();
            var bucket = storage.CreateBucket(_fixture.ProjectId, $"dlp_dotnet_test_bucket_samples_{random.Next()}");

            // Upload the file to cloud storage.
            var filePath = Path.Combine(_fixture.ResourcePath, "test.txt");
            using var fileStream = File.OpenRead(filePath);
            storage.UploadObject(bucket.Name, "test.txt", null, fileStream);

            try
            {
                var result = InspectStorageWithSCCIntegration.SendGcsData(_fixture.ProjectId, $"gs://{bucket.Name}");
                JobsDelete.DeleteJob(result.Name);
            }
            finally
            {
                storage.DeleteBucket(bucket.Name, new DeleteBucketOptions { DeleteObjects = true });
            }
        }
    }
}
