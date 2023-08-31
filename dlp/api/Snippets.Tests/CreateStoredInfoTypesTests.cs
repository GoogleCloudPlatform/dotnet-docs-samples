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
using Xunit;
using System;

namespace GoogleCloudSamples
{
    public class CreateStoredInfoTypesTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public CreateStoredInfoTypesTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestCreate()
        {
            Random random = new Random();
            // Create a bucket.
            var storage = StorageClient.Create();
            var bucket = storage.CreateBucket(_fixture.ProjectId, $"dlp_dotnet_test_bucket_samples_{random.Next()}");

            var dlp = DlpServiceClient.Create();
            try
            {
                var storedInfoTypeId = $"github-usernames-create-{random.Next()}";
                var result = CreateStoredInfoTypes.Create(_fixture.ProjectId, $"gs://{bucket.Name}", storedInfoTypeId);
                Assert.Contains(storedInfoTypeId, result.Name);

                // Delete the created stored infoTypes.
                dlp.DeleteStoredInfoType(new DeleteStoredInfoTypeRequest
                {
                    Name = result.Name
                });
            }
            finally
            {
                // Delete the created bucket.
                storage.DeleteBucket(bucket.Name, new DeleteBucketOptions { DeleteObjects = true });
            }
        }
    }
}
