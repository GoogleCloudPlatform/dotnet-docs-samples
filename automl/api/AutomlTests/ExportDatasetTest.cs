// Copyright 2020 Google LLC
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

using Grpc.Core;
using System;
using System.IO;
using System.Threading;
using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class ExportDatasetTest
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _datasetId;
        public ExportDatasetTest(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _datasetId = "TEN0000000000000000000";
        }

        [Fact]
        public void TestExportDataset()
        {
            try
            {
                // export dataset
                ConsoleOutput output = _fixture.SampleRunner.Run("export_dataset", _fixture.ProjectId, _datasetId,
                  $"gs://{ _fixture.Bucket.Name}/TEST_EXPORT_OUTPUT/");

                Assert.Contains("The Dataset doesn't exist or is inaccessible for use with AutoMl.", output.Stdout);
            }
            catch (Exception ex) when (ex is ThreadInterruptedException ||
   ex is IOException || ex is RpcException || ex is AggregateException)
            {
                Assert.Contains("The Dataset doesn't exist or is inaccessible for use with AutoMl.", ex.Message);
            }
        }
    }
}
