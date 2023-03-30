// Copyright 2022 Google Inc.
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
public class ExecutePartitionedDmlAsyncPostgresTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly ExecutePartitionedDmlAsyncPostgresSample _sample;

    public ExecutePartitionedDmlAsyncPostgresTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new ExecutePartitionedDmlAsyncPostgresSample();
    }

    [Fact]
    public async Task TestExecutePartitionedDmlAsyncPostgres()
    {
        // Arrange.
        // Insert data that cannot be inserted by any other tests to avoid errors.
        await InsertDataAsync();

        // Act.
        var result = await _sample.ExecutePartitionedDmlAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);
        Assert.Equal(2, result);
    }

    private async Task InsertDataAsync()
    {
        SpannerBatchCommand batchCommand = _spannerFixture.PgSpannerConnection.CreateBatchDmlCommand();
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (16, 'Elvis', 'Presley')");
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (17, 'John', 'Lennon')");

        await batchCommand.ExecuteNonQueryAsync();
    }
}
