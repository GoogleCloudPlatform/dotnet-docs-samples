// Copyright 2024 Google Inc.
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

// [START spanner_create_database_with_MR_CMEK]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CreateDatabaseWithMultiRegionEncryptionAsyncSample
{
    public async Task<Database> CreateDatabaseWithMultiRegionEncryptionAsync(string projectId, string instanceId, string databaseId, IEnumerable<CryptoKeyName> kmsKeyNames)
    {
        // Create a DatabaseAdminClient instance that can be used to execute a
        // CreateDatabaseRequest with custom encryption configuration options.
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();
        // Define create table statement for table #1.
        var createSingersTable =
            @"CREATE TABLE Singers (
                SingerId INT64 NOT NULL,
                FirstName STRING(1024),
                LastName STRING(1024),
                ComposerInfo BYTES(MAX)
            ) PRIMARY KEY (SingerId)";
        // Define create table statement for table #2.
        var createAlbumsTable =
            @"CREATE TABLE Albums (
                SingerId INT64 NOT NULL,
                AlbumId INT64 NOT NULL,
                AlbumTitle STRING(MAX)
             ) PRIMARY KEY (SingerId, AlbumId),
             INTERLEAVE IN PARENT Singers ON DELETE CASCADE";

        // Create the CreateDatabase request with encryption configuration and execute it.
        var request = new CreateDatabaseRequest
        {
            ParentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId),
            CreateStatement = $"CREATE DATABASE `{databaseId}`",
            ExtraStatements = { createSingersTable, createAlbumsTable },
            EncryptionConfig = new EncryptionConfig
            {
                KmsKeyNamesAsCryptoKeyNames = { kmsKeyNames },
            },
        };
        var operation = await databaseAdminClient.CreateDatabaseAsync(request);

        // Wait until the operation has finished.
        Console.WriteLine("Waiting for the operation to finish.");
        var completedResponse = await operation.PollUntilCompletedAsync();
        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating database: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        var database = completedResponse.Result;
        Console.WriteLine($"Database {database.Name} created with encryption keys {string.Join(", ", kmsKeyNames)}");

        return database;
    }
}
// [END spanner_create_database_with_MR_CMEK]
