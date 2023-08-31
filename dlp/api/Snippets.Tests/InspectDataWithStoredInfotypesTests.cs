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
using System.Runtime.Intrinsics.Arm;
using System.Threading;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectDataWithStoredInfotypesTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public InspectDataWithStoredInfotypesTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestInspect()
        {
            // Create the bucket for creating stored infoType.
            Random random = new Random();
            var storage = StorageClient.Create();
            var bucket = storage.CreateBucket(_fixture.ProjectId, $"dlp_dotnet_test_bucket_samples_{random.Next()}");

            try
            {
                // Create Stored InfoType first.
                var storedInfoTypeId = $"github-usernames-inspect-{random.Next()}";
                var response = CreateStoredInfoTypes.Create(_fixture.ProjectId, $"gs://{bucket.Name}", storedInfoTypeId);
                Thread.Sleep(TimeSpan.FromSeconds(30)); // Sleep until created stored infotype gets ready.

                var githubUser = "jamesC311";
                var text = $"Commit was made by {githubUser}.";
                var result = InspectDataWithStoredInfotypes.Inspect(_fixture.ProjectId, response.Name, text);
                Assert.Single(result.Result.Findings);
                Assert.Contains(result.Result.Findings, f => f.InfoType.Name == "GITHUB_LOGINS");
                Assert.Contains(result.Result.Findings, f => f.Quote == githubUser);

                var dlp = DlpServiceClient.Create();

                // Delete the created stored infoTypes
                dlp.DeleteStoredInfoType(new DeleteStoredInfoTypeRequest
                {
                    Name = response.Name
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
