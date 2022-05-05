﻿// Copyright 2022 Google Inc.
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

using Google.Cloud.Spanner.Data;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class ExecutePartitionedDmlAsyncPostgreTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly ExecutePartitionedDmlAsyncPostgreSample _sample;

    public ExecutePartitionedDmlAsyncPostgreTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new ExecutePartitionedDmlAsyncPostgreSample();
    }

    [Fact]
    public async Task TestExecutePartitionedDmlAsyncPostgre()
    {
        // Arrange.
        // Insert data that cannot be inserted by any other tests to avoid errors.
        await InsertDataAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);
        
        // Act.
        var result = await _sample.ExecutePartitionedDmlAsyncPostgre(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);
        Assert.Equal(2, result);
    }

    private async Task InsertDataAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        SpannerBatchCommand batchCommand = connection.CreateBatchDmlCommand();
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (12, 'Elvis', 'Presley')");
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (13, 'John', 'Lennon')");

        await batchCommand.ExecuteNonQueryAsync();
    }
}
