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

using Grpc.Core;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class CreateInstanceTest
{
    private readonly SpannerFixture _spannerFixture;

    public CreateInstanceTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public void TestCreateInstance()
    {
        CreateInstanceSample createInstanceSample = new CreateInstanceSample();
        // Instance already exists since it was created in the test setup so it should throw an exception.
        var exception = Assert.Throws<RpcException>(() => createInstanceSample.CreateInstance(_spannerFixture.ProjectId, _spannerFixture.InstanceId));
        Assert.Equal(StatusCode.AlreadyExists, exception.StatusCode);
    }
}
