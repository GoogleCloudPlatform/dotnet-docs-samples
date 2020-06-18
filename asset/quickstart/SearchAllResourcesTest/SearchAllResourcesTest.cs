/*
 * Copyright (c) 2020 Google LLC.
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

using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using System;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace GoogleCloudSamples
{
    public class BigQueryClientFixture : IDisposable
    {
        private readonly BigQueryClient _client = BigQueryClient.Create(Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"));

        public BigQueryClientFixture()
        {
            DatasetId = RandomDatasetId();
            var dataset = new Dataset
            {
                Location = "US"
            };
            _client.CreateDataset(datasetId: DatasetId, dataset);
        }

        public void Dispose()
        {
            _client.DeleteDataset(datasetId: DatasetId);
        }

        private static string RandomDatasetId()
        {
            return TestUtil.RandomName();
        }

        public string DatasetId { get; private set; }
    }

    public class SearchAllResourcesTest : IClassFixture<BigQueryClientFixture>
    {
        static readonly CommandLineRunner s_runner = new CommandLineRunner
        {
            Command = "SearchAllResources",
            VoidMain = SearchAllResources.Main,
        };
        private readonly ITestOutputHelper _testOutput;
        private readonly string _datasetId;
        public SearchAllResourcesTest(ITestOutputHelper output, BigQueryClientFixture bigQueryFixture)
        {
            _testOutput = output;
            _datasetId = bigQueryFixture.DatasetId;
        }

        [Fact]
        public void TestSearchAllResources()
        {
            Environment.SetEnvironmentVariable("SCOPE-OF-THE-SEARCH", "projects/" + Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"));
            Environment.SetEnvironmentVariable("QUERY-STATEMENT", "name:" + _datasetId);
            Thread.Sleep(10000);
            var output = s_runner.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Contains(_datasetId, output.Stdout);
        }
    }
}
