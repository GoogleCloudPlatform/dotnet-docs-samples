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
public class AddJsonbColumnAsyncPostgresTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly AddJsonbColumnAsyncPostgresSample _sample;

    public AddJsonbColumnAsyncPostgresTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new AddJsonbColumnAsyncPostgresSample();
    }

    [Fact]
    public async Task TestAddJsonbColumnAsyncPostgres()
    {
        await _spannerFixture.RunWithTemporaryPostgresDatabaseAsync(async databaseId =>
        {
            // Arrange - Create VenueDetails table.
            await CreateVenueDetailsTable(databaseId);
            // Act - Add a JSONB column to the VenueDetails table.
            await _sample.AddJsonbColumnAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
            // If we reach here without error, we are good. 
        });
    }

    private async Task CreateVenueDetailsTable(string databaseId)
    {
        string connectionString = $"Data Source=projects/{_spannerFixture.ProjectId}/instances/{_spannerFixture.InstanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);

        // Define create table statement for VenueDetails.
        const string createVenueDetailsTableStatement =
        @"CREATE TABLE VenueDetails (
            VenueId BIGINT NOT NULL PRIMARY KEY,
            VenueName VARCHAR(1024))";

        using var cmd = connection.CreateDdlCommand(createVenueDetailsTableStatement);
        await cmd.ExecuteNonQueryAsync();
    }
}
