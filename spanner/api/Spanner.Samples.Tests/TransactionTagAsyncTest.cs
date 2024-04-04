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
public class TransactionTagAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public TransactionTagAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestTransactionTagAsync()
    {
        var sample = new TransactionTagAsyncSample();
        var modifiedCount = await sample.TransactionTagAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId,
            _spannerFixture.DatabaseId);
        // At least a row has been modified and a few may have been updated.
        Assert.InRange(modifiedCount, 1, int.MaxValue);
    }
}
