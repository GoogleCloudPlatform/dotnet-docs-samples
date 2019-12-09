// Copyright 2019 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using Xunit;

namespace GoogleCloudSamples
{
    [CollectionDefinition(nameof(AutoMLFixture))]
    public class AutoMLFixture : ICollectionFixture<AutoMLFixture>
    {
        private readonly StorageClient _client;
        public readonly string ProjectId;
        public readonly Bucket Bucket;

        public CommandLineRunner SampleRunner { get; } = new CommandLineRunner()
        {
            Main = AutoMLProgram.Main
        };

        public AutoMLFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            _client = StorageClient.Create();
            string BucketName = "automl-bucket-" + TestUtil.RandomName();
            Bucket bucket = new Bucket { Location = "us-central1", Name = BucketName };
            Bucket = _client.CreateBucket(ProjectId, bucket);
        }

        public void Dispose()
        {
            _client.DeleteBucket(Bucket.Name,
            new DeleteBucketOptions
            {
                DeleteObjects = true
            });
            _client.Dispose();
        }
    }
}
