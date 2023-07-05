// Copyright 2023 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class UpdateDatabaseAsyncSampleTest
{
    private readonly SpannerFixture _spannerFixture;

    public UpdateDatabaseAsyncSampleTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task UpdateDatabaseAsyncSample()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            var sample = new UpdateDatabaseAsyncSample();
            var updatedDatabase = await sample.UpdateDatabaseAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            Assert.True(updatedDatabase.EnableDropProtection);

            // Removing database protection after successfull test so that the temporary database can be dropped.
            await DisableDropDbProtectionAsync(updatedDatabase);
        });
    }

    private async Task DisableDropDbProtectionAsync(Database database)
    {
        var databaseAdminClient = await DatabaseAdminClient.CreateAsync();
        database.EnableDropProtection = false;

        var operation = await databaseAdminClient.UpdateDatabaseAsync(
            new UpdateDatabaseRequest()
            {
                Database = database,
                UpdateMask = new FieldMask { Paths = { "enable_drop_protection" } }
            });

        // Wait until the operation has finished.
        var completedResponse = await operation.PollUntilCompletedAsync();

        if (completedResponse.IsFaulted)
        {
            throw completedResponse.Exception;
        }
    }
}

