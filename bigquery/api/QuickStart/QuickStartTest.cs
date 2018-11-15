/*
 * Copyright (c) 2018 Google LLC
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
using System.Collections.Generic;
using System.Linq;
using Google.Cloud.BigQuery.V2;
using Xunit;

namespace GoogleCloudSamples
{
    public class QuickStartTest : IDisposable
    {
        private readonly BigQueryClient _client;
        private readonly string _projectId;
        readonly List<string> _tempDatasetIds = new List<string>();

        public QuickStartTest()
        {
            _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            _client = BigQueryClient.Create(_projectId);
        }
        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            VoidMain = Program.Main,
            Command = "dotnet run"
        };

        [Fact]
        public void TestRunQuickStart()
        {
            string expectedOutputFirstWord = "Dataset";
            string expectedOutputLastWord = "created.";
            _tempDatasetIds.Add("my_new_dataset");
            var output = _quickStart.Run();
            Assert.Equal(0, output.ExitCode);
            var outputParts = output.Stdout.Split(new[] { ' ' });
            Assert.Equal(expectedOutputFirstWord, outputParts.First().Trim());
            Assert.Equal(expectedOutputLastWord, outputParts.Last().Trim());
        }

        public void Dispose()
        {
            foreach (string datasetId in _tempDatasetIds)
            {
                _client.DeleteDataset(
                    datasetId,
                    new DeleteDatasetOptions() { DeleteContents = true }
                );
            }
        }
    }
}
