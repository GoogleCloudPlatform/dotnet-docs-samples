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

using Google.Cloud.AutoML.V1;
using Google.LongRunning;
using System.Linq;
using Xunit;

[Collection(nameof(AutoMLFixture))]
public class GetOperationStatusTest
{
    private readonly AutoMLFixture _fixture;
    private readonly AutoMLGetOperationStatus _sample;

    public GetOperationStatusTest(AutoMLFixture fixture)
    {
        _fixture = fixture;
        _sample = new AutoMLGetOperationStatus();
    }

    [Fact]
    public void TestOperationStatus()
    {
        // Get operation ID
        var listOperSample = new AutoMLListOperationStatus();

        Operation firstOper = listOperSample.ListOperationStatus(_fixture.ProjectId, "us-central1").First();
        // Act
        Operation<Model, OperationMetadata> result = _sample.GetOperationStatus(firstOper.Name);

        // Assert 
        Assert.Contains(firstOper.Name, result.Name);
    }
}
