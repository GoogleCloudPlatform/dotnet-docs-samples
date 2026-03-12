// Copyright 2026 Google LLC
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

using System;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class BatchWriteAtLeastOnceAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public BatchWriteAtLeastOnceAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestBatchWriteAtLeastOnceAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            // 1. Create tables
            await _spannerFixture.CreateSingersAndAlbumsTableAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            // 2. Run the BatchWrite sample
            BatchWriteAtLeastOnceAsyncSample sample = new BatchWriteAtLeastOnceAsyncSample();
            await sample.BatchWriteAtLeastOnceAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            // 3. Verify data was written (optional but good practice)
            // We can query the database to ensurerows were inserted.
            using var connection = new Google.Cloud.Spanner.Data.SpannerConnection(
                $"Data Source=projects/{_spannerFixture.ProjectId}/instances/{_spannerFixture.InstanceId}/databases/{databaseId}");
            await connection.OpenAsync();

            using var cmd = connection.CreateSelectCommand("SELECT COUNT(*) FROM Singers");
            var count = await cmd.ExecuteScalarAsync<long>();
            Assert.True(count >= 3, $"Expected at least 3 singers, found {count}");

            using var cmdAlbums = connection.CreateSelectCommand("SELECT COUNT(*) FROM Albums");
            var countAlbums = await cmdAlbums.ExecuteScalarAsync<long>();
            Assert.True(countAlbums >= 2, $"Expected at least 2 albums, found {countAlbums}");
        });
    }
}
