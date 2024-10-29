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

using Google.Cloud.Spanner.Admin.Instance.V1;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class CreateInstanceWithProcessingUnitsTest
{
    private readonly SpannerFixture _spannerFixture;

    public CreateInstanceWithProcessingUnitsTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestCreateInstanceWithProcessingUnits()
    {
        var createInstanceWithProcessingUnitsSample = new CreateInstanceWithProcessingUnitsSample();

        Instance instance = await _spannerFixture.SafeAdminAsync(() => Task.FromResult(
            createInstanceWithProcessingUnitsSample.CreateInstanceWithProcessingUnits(
                _spannerFixture.ProjectId, _spannerFixture.InstanceIdWithProcessingUnits)));

        Assert.Equal(500, instance.ProcessingUnits);
    }
}
