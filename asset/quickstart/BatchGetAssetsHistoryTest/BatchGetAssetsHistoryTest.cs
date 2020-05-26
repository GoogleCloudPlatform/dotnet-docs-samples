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

using System;
using Xunit;
using Xunit.Abstractions;

namespace GoogleCloudSamples
{
    public class BatchGetAssetsHistoryTest : IClassFixture<RandomBucketFixture>
    {
        static readonly CommandLineRunner s_runner = new CommandLineRunner
        {
            Command = "BatchGetAssetsHistory",
            VoidMain = BatchGetAssetsHistory.Main,
        };
        private readonly ITestOutputHelper _testOutput;
        private readonly string _bucketName;
        public BatchGetAssetsHistoryTest(ITestOutputHelper output, RandomBucketFixture bucketFixture)
        {
            _testOutput = output;
            _bucketName = bucketFixture.BucketName;
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/692")]
        public void TestBatchGetAssetsHistory()
        {
            string assetName = String.Format("//storage.googleapis.com/{0}", _bucketName);
            Environment.SetEnvironmentVariable("ASSET_NAME", assetName);
            var output = s_runner.Run();
            _testOutput.WriteLine(output.Stdout);
            string expectedOutput = assetName;
            if (output.Stdout.Length > 0)
            {
                Assert.Contains(expectedOutput, output.Stdout);
            }
        }
    }
}

