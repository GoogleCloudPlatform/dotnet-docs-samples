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

using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class InsertUsingDmlReturningAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public InsertUsingDmlReturningAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestInsertUsingDmlReturningAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            await CreateTableAndInsertData(databaseId);
            InsertUsingDmlReturningAsyncSample sample = new InsertUsingDmlReturningAsyncSample();
            var insertedSingerNames = await sample.InsertUsingDmlReturningAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            Assert.Equal(4, insertedSingerNames.Count);
            Assert.Contains("Melissa Garcia", insertedSingerNames);
            Assert.Contains("Russell Morales", insertedSingerNames);
            Assert.Contains("Jacqueline Long", insertedSingerNames);
            Assert.Contains("Dylan Shaw", insertedSingerNames);
        });
    }

    private async Task CreateTableAndInsertData(string databaseId)
    {
        await _spannerFixture.CreateSingersAndAlbumsTableAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
        var insertDataAsyncSample = new InsertDataAsyncSample();
        await insertDataAsyncSample.InsertDataAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
    }
}
