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

using Xunit;

[Collection(nameof(RedisFixture))]
public class GetInstanceTest
{
    private readonly RedisFixture _fixture;
    private readonly GetInstanceSample _sample;

    public GetInstanceTest(RedisFixture fixture)
    {
        _fixture = fixture;
        _sample = new GetInstanceSample();
    }

    [Fact]
    public void GetInstance()
    {
        //run the sample code.
        var result = _sample.GetInstance(_fixture.ProjectId, _fixture.LocationId, _fixture.InstanceId);

        Assert.Equal(result.InstanceName.InstanceId, _fixture.InstanceId);
    }
}
