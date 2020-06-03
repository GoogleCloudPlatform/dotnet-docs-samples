// Copyright 2020 Google LLC
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

using System.Linq;
using Xunit;

[Collection(nameof(RedisFixture))]
public class ListInstanceTest
{
    private readonly RedisFixture _fixture;
    private readonly ListInstanceSample _sample;
    public ListInstanceTest(RedisFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListInstanceSample();
    }

    [Fact]
    public void ListInstance()
    {
        //run the sample code.
        var result = _sample.ListInstance(_fixture.ProjectId, _fixture.LocationId);

        Assert.True(result.ToList().Count > 0);
    }
}
