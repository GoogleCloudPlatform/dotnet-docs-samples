// Copyright 2020 Google Inc.
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
public class DeleteUsingPartitionedDmlCoreAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public DeleteUsingPartitionedDmlCoreAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestDeleteUsingPartitionedDmlCoreAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            await InsertDataAsync(databaseId);
            DeleteUsingPartitionedDmlCoreAsyncSample sample = new DeleteUsingPartitionedDmlCoreAsyncSample();
            var rowCount = await sample.DeleteUsingPartitionedDmlCoreAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
            Assert.Equal(2, rowCount);
        }, SpannerFixture.CreateSingersTableStatement, SpannerFixture.CreateAlbumsTableStatement);
    }

    private async Task InsertDataAsync(string databaseId)
    {
        using var connection = _spannerFixture.GetConnection(databaseId);
        var command = connection.CreateDmlCommand("INSERT INTO Singers (SingerId, FirstName, LastName) " +
                           "VALUES ($1, $2, $3), " +
                           "($4, $5, $6)",
                           new SpannerParameterCollection
                           {
                                        { "p1", SpannerDbType.Int64, 15 },
                                        { "p2", SpannerDbType.String, "Bruce" },
                                        { "p3", SpannerDbType.String, "Allison" },
                                        { "p4", SpannerDbType.Int64, 16 },
                                        { "p5", SpannerDbType.String, "George" },
                                        { "p6", SpannerDbType.String, "Harrison" },
                           });
        await command.ExecuteNonQueryAsync();
    }
}
