// Copyright 2019 Google Inc. All Rights Reserved.
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

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using Xunit;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Fixture which is set up at the start of the test run, and torn down at the end.
    /// This creates new buckets with test data, and deletes them at the end of the test.
    /// Also, it creates glossary for translation and delete that glossary at the end of test.
    /// </summary>

    [CollectionDefinition(nameof(TranslateFixture))]
    public sealed class TranslateFixture : IDisposable, ICollectionFixture<TranslateFixture>
    {
        private readonly string _glossaryInputUri = "gs://cloud-samples-data/translation/glossary_ja.csv";
        private readonly StorageClient _client;

        public readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        public string _glossaryId;
        public readonly string _modelId = "TRL8772189639420149760";
        public string _bucketName;

        public TranslateFixture()
        {
            _client = StorageClient.Create();
            _bucketName = "translate-v3-bucket-" + TestUtil.RandomName();
            _glossaryId = "must-start-with-letters" + TestUtil.RandomName();

            CreateBucket(_bucketName);
            CreateGlossary.CreateGlossarySample(_projectId, _glossaryId, _glossaryInputUri);
        }

        internal Bucket CreateBucket(string name)
        {
            var bucket = _client.CreateBucket(_projectId, _bucketName);
            return bucket;
        }


        public void Dispose()
        {
            using (var storageClient = StorageClient.Create())
            {
                storageClient.DeleteBucket(_bucketName,
                new DeleteBucketOptions
                {
                    DeleteObjects = true
                });
            }
            DeleteGlossary.DeleteGlossarySample(_projectId, _glossaryId);
        }
    }
}
