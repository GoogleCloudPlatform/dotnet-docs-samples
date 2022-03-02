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
public class QueryUsingParametersAsyncPostgreTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly QueryUsingParametersAsyncPostgreSample _sample;

    public QueryUsingParametersAsyncPostgreTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new QueryUsingParametersAsyncPostgreSample();
    }

    [Fact]
    public async Task TestQueryUsingParametersAsyncPostgre()
    {
        //Arrange. 
        // Insert data that cannot be inserted by any other tests to avoid errors.
        await InsertDataAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        // Act.
        var result = await _sample.QueryUsingParametersAsyncPostgre(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        //Assert.
        Assert.Single(result);
    }

    private async Task InsertDataAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();
        var command = connection.CreateDmlCommand("INSERT INTO Singers(SingerId, FirstName, LastName) VALUES(10, 'Sonu', 'Nigam')");
        await command.ExecuteNonQueryAsync();
    }
}
