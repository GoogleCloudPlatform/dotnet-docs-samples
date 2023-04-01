// Copyright 2022 Google Inc.
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
public class IdentifierCaseSensitivityAsyncPostgresTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly IdentifierCaseSensitivityAsyncPostgresSample _sample;

    public IdentifierCaseSensitivityAsyncPostgresTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new IdentifierCaseSensitivityAsyncPostgresSample();
    }

    [Fact]
    public async Task TestIdentifierCaseSensitivityAsyncPostgres()
    {
        // Act.
        var count = await _sample.IdentifierCaseSensitivityAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        //Assert.
        // The last insert operation inserts one row. If we reach till the last insert statement without any issue,
        // row inserted count of one should be returned.
        Assert.Equal(1, count);
    }
}
