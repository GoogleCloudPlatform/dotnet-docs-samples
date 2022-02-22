/*
 * Copyright 2022 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Api.Gax;
using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(BigQueryStorageFixture))]
public class AppendRowsPendingTests
{
    private readonly BigQueryStorageFixture _fixture;

    private readonly AppendRowsPendingSample _sample;

    private readonly List<string> _testData = new List<string> { "Alice", "Bob", "Charles" };

    public AppendRowsPendingTests(BigQueryStorageFixture bigQueryStorageFixture)
    {
        _fixture = bigQueryStorageFixture;
        _sample = new AppendRowsPendingSample();
    }

    [Fact]
    public async Task TestAppendRows()
    {
        // Arrange: Ensure dataset and table exists.
        string datasetId = $"dataset{Guid.NewGuid().ToString("N").Substring(0, 18)}"; // - is not allowed in name
        string tableName = "append_row_pending";
        TableSchema tableSchema = new TableSchemaBuilder
        {
            { "customer_number", BigQueryDbType.Int64 },
            { "customer_name", BigQueryDbType.String }
        }.Build();

        _fixture.EnsureDataSetAndTableExists(datasetId, tableName, tableSchema);

        // Act: Append Rows in new database.
        await _sample.AppendRowsPendingAsync(_fixture.ProjectId, datasetId, tableName);

        // Get the inserted rows in table.
        PagedEnumerable<TableDataList, BigQueryRow> insertedData = _fixture.Client.ListRows(datasetId, tableName);

        // Assert: Check that inserted data matches the test data.
        Assert.Collection(insertedData,
            item1 => Assert.Equal(_testData[0], item1["customer_name"]),
            item2 => Assert.Equal(_testData[1], item2["customer_name"]),
            item3 => Assert.Equal(_testData[2], item3["customer_name"]));
    }
}
