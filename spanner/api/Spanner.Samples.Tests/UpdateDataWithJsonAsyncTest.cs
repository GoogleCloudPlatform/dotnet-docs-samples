// Copyright 2021 Google Inc.
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
public class UpdateDataWithJsonAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public UpdateDataWithJsonAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestUpdateDataWithJsonAsync()
    {
        await _spannerFixture.CreateVenuesTableAndInsertDataAsync();

        var addColumnSample = new AddJsonColumnAsyncSample();
        await addColumnSample.AddJsonColumnAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId);

        var updateJsonSample = new UpdateDataWithJsonAsyncSample();
        await updateJsonSample.UpdateDataWithJsonAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId);

        await _spannerFixture.DeleteVenuesTable();
    }
}
