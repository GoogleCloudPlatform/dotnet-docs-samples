/*
 * Copyright (c) 2018 Google LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using Xunit;
using Xunit.Abstractions;

namespace GoogleCloudSamples
{
    public class ExportAssetsTest : IClassFixture<RandomBucketFixture>
    {
        static readonly CommandLineRunner s_runner = new CommandLineRunner
        {
            Command = "ExportAssets",
            VoidMain = ExportAssets.Main,
        };
        private readonly ITestOutputHelper _testOutput;
        private readonly StorageClient _storageClient = StorageClient.Create();
        private readonly string _bucketName;
        public ExportAssetsTest(ITestOutputHelper output, RandomBucketFixture bucketFixture)
        {
            _testOutput = output;
            _bucketName = bucketFixture.BucketName;
        }

        [Fact]
        public void TestExportAssets()
        {
            Environment.SetEnvironmentVariable("AssetBucketName", _bucketName);
            var output = s_runner.Run();
            _testOutput.WriteLine(output.Stdout);
            string expectedOutput = String.Format("\"outputConfig\": {{ \"gcsDestination\": {{ \"uri\": \"gs://{0}/my-assets.txt\" }} }}", _bucketName);
            Assert.Contains(expectedOutput, output.Stdout);
        }
    }
}

