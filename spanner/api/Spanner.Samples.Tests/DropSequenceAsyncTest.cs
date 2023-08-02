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
// limitations under the License.

using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.Cloud.Spanner.Data;
using Google.Cloud.Storage.V1;
using Grpc.Core;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class DropSequenceAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public DropSequenceAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestDropSequenceAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            var createSequenceSample = new CreateSequenceSample();
            await createSequenceSample.CreateSequenceAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            var sample = new DropSequenceSample();
            await sample.DropSequenceSampleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            string connectionString = $"Data Source=projects/{_spannerFixture.ProjectId}/instances/{_spannerFixture.InstanceId}/databases/{databaseId}";
            using var connection = new SpannerConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = connection.CreateDmlCommand(
                @"INSERT INTO Customers (CustomerName) VALUES ('Alice'), ('David'), ('Marc') THEN RETURN CustomerId");

            var exception = await Assert.ThrowsAsync<SpannerException>(async () => await cmd.ExecuteReaderAsync());
        });
    }

    [Fact]
    public async Task TestDropSequencePostgresqlAsync()
    {
        await _spannerFixture.RunWithPostgresqlTemporaryDatabaseAsync(async databaseId =>
        {
            var createSequenceSample = new CreateSequencePostgresqlSample();
            await createSequenceSample.CreateSequencePostgresqlSampleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            var sample = new DropSequenceSample();
            await sample.DropSequenceSampleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            string connectionString = $"Data Source=projects/{_spannerFixture.ProjectId}/instances/{_spannerFixture.InstanceId}/databases/{databaseId}";
            using var connection = new SpannerConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = connection.CreateDmlCommand(
                @"INSERT INTO Customers (CustomerName) VALUES ('Alice'), ('David'), ('Marc') RETURNING CustomerId");

            var exception = await Assert.ThrowsAsync<SpannerException>(async () => await cmd.ExecuteReaderAsync());
        });
    }
}
