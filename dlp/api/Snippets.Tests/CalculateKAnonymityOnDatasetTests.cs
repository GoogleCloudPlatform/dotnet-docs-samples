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

using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class CalculateKAnonymityOnDatasetTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public CalculateKAnonymityOnDatasetTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestCreate()
        {
            Random random = new Random();
            BigQueryClient bigQueryClient = BigQueryClient.Create(_fixture.ProjectId);
            var datasetId = $"dlp_test_dotnet_dataset_{random.Next()}";
            var inputTableId = $"dlp_test_dotnet_table_{random.Next()}";
            var outputTableId = $"dlp_test_dotnet_output_table_{random.Next()}";

            // Create the dataset.
            var dataset = new Dataset
            {
                Location = "US"
            };
            var datasetResponse = bigQueryClient.CreateDataset(datasetId, dataset);
            var schema = new TableSchemaBuilder
            {
                { "Name", BigQueryDbType.String },
                { "TelephoneNumber", BigQueryDbType.String },
                { "Mystery", BigQueryDbType.String },
                { "Age", BigQueryDbType.Int64 },
                { "Gender", BigQueryDbType.String },
            }.Build();

            datasetResponse.CreateTable(inputTableId, schema: schema);

            // Insert the rows inside the table which we have created above.
            BigQueryInsertRow[] rows = new BigQueryInsertRow[]
            {
                new BigQueryInsertRow(insertId: "row1") {
                    { "Name", "James" },
                    { "TelephoneNumber", "(567) 890-1234" },
                    { "Mystery", "8291 3627 8250 1234" },
                    { "Age", "19" },
                    { "Gender", "Male" }
                },
                new BigQueryInsertRow(insertId: "row2") {
                    { "Name", "Gandalf" },
                    { "TelephoneNumber", "(223) 456-7890" },
                    { "Mystery", "4231 5555 6781 9876"},
                    { "Age", "27 " },
                    { "Gender", "Male" }
                },
                new BigQueryInsertRow(insertId: "row3") {
                    { "Name", "Dumbledore" },
                    { "TelephoneNumber", "(313) 337-1337" },
                    { "Mystery", "6291 8765 1095 7629"},
                    { "Age", "27 " },
                    { "Gender", "Male" }
                },
                new BigQueryInsertRow(insertId: "row4") {
                    { "Name", "Joe" },
                    { "TelephoneNumber", "(452) 223-1234" },
                    { "Mystery", "3782 2288 1166 3030"},
                    { "Age", "35" },
                    { "Gender", "Male" }
                },
                new BigQueryInsertRow(insertId: "row5") {
                    { "Name", "Marie" },
                    { "TelephoneNumber", "(452) 223-1234" },
                    { "Mystery", "8291 3627 8250 1234"},
                    { "Age", "35" },
                    { "Gender", "Female" }
                },
            };
            bigQueryClient.InsertRows(datasetId, inputTableId, rows);

            try
            {
                var response = CalculateKAnonymityOnDataset.CalculateKAnonymitty(
                    _fixture.ProjectId,
                    datasetId,
                    inputTableId,
                    outputTableId);

                // Delete the created job.
                JobsDelete.DeleteJob(response.Name);
            }
            finally
            {
                bigQueryClient.DeleteDataset(datasetId, new DeleteDatasetOptions { DeleteContents = true });
            }
        }
    }
}