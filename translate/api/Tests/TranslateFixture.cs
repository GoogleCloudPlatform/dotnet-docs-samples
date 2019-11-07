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

        public string ProjectId { get; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        public string ModelId { get; } = "TRL8772189639420149760";
        public string GlossaryId { get; }
        public string BucketName { get; }

        public CommandLineRunner SampleRunner { get; } = new CommandLineRunner()
        {
            VoidMain = TranslateV3Samples.Main
        };

        public TranslateFixture()
        {
            _client = StorageClient.Create();
            BucketName = "translate-v3-bucket-" + TestUtil.RandomName();
            GlossaryId = "must-start-with-letters" + TestUtil.RandomName();

            _client.CreateBucket(ProjectId, BucketName);
            CreateGlossary.CreateGlossarySample(ProjectId, GlossaryId, _glossaryInputUri);
        }

        public void Dispose()
        {
            _client.DeleteBucket(BucketName,
            new DeleteBucketOptions
            {
                DeleteObjects = true
            });
            _client.Dispose();
            DeleteGlossary.DeleteGlossarySample(ProjectId, GlossaryId);
        }
    }
}
