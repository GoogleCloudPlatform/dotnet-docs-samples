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

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class QueryDataWithTimestampColumnAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public QueryDataWithTimestampColumnAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestQueryDataWithTimestampColumnAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            await _spannerFixture.InitializeTempDatabaseAsync(databaseId);
            AddCommitTimestampAsyncSample addCommitTimestampAsyncSample = new AddCommitTimestampAsyncSample();
            await addCommitTimestampAsyncSample.AddCommitTimestampAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            QueryDataWithTimestampColumnAsyncSample sample = new QueryDataWithTimestampColumnAsyncSample();
            UpdateDataWithTimestampColumnAsyncSample updateWithTimestamp = new UpdateDataWithTimestampColumnAsyncSample();
            await updateWithTimestamp.UpdateDataWithTimestampColumnAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
            var albums = await sample.QueryDataWithTimestampColumnAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
            Assert.Contains(albums, a => a.AlbumId == 1 && a.SingerId == 1 && a.LastUpdateTime != null);
            Assert.Contains(albums, a => a.AlbumId == 2 && a.SingerId == 2 && a.LastUpdateTime != null);
        }, SpannerFixture.CreateSingersTableStatement, SpannerFixture.CreateAlbumsTableStatement);
    }
}
