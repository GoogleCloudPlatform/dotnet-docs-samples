// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class CreateDatabaseWithQueueAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public CreateDatabaseWithQueueAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestCreateDatabaseWithQueueAsync()
    {
        var databaseId = _spannerFixture.GenerateTempDatabaseId("my-queue-db-");

        CreateDatabaseWithQueueAsyncSample sample = new CreateDatabaseWithQueueAsyncSample();
        await sample.CreateDatabaseWithQueueAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

        GetDatabaseDdlAsyncSample getDatabaseDdlAsyncSample = new GetDatabaseDdlAsyncSample();
        var response = await getDatabaseDdlAsyncSample.GetDatabaseDdlAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
        Assert.Collection(response.Statements,
            // Only check the start of the statement, as there is no guarantee on exactly
            // how Cloud Spanner will format the returned SQL string.
            statement => Assert.StartsWith("CREATE QUEUE MyQueue", statement)
        );
    }
}
