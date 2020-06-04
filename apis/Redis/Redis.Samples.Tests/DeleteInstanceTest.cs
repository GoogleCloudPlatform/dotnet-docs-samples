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

using Grpc.Core;
using System;
using Xunit;

[Collection(nameof(RedisFixture))]
public class DeleteInstanceTest
{
    private readonly RedisFixture _fixture;
    public DeleteInstanceTest(RedisFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void DeleteInstance()
    {
        //create new instance.
        var instanceId = $"csharp-{Guid.NewGuid().ToString().Substring(0, 20)}";
        CreateInstanceSample createInstanceSample = new CreateInstanceSample();
        createInstanceSample.CreateInstance(_fixture.ProjectId, _fixture.LocationId, instanceId);

        DeleteInstanceSample deleteInstanceSample = new DeleteInstanceSample();
        deleteInstanceSample.DeleteInstance(_fixture.ProjectId, _fixture.LocationId, instanceId);

        try
        {
            GetInstanceSample getInstanceSample = new GetInstanceSample();
            //should throw exception.
            var instance = getInstanceSample.GetInstance(_fixture.ProjectId, _fixture.LocationId, instanceId);
        }
        catch (Exception exception)
        {
            Assert.Equal(StatusCode.NotFound, ((RpcException)exception).StatusCode);
        }
    }
}
