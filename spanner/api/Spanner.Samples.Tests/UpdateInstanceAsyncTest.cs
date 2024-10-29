// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

using Google.Cloud.Spanner.Admin.Instance.V1;
using System.Threading.Tasks;
using Xunit;

namespace Spanner.Samples.Tests;

[Collection(nameof(SpannerFixture))]
public class UpdateInstanceAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public UpdateInstanceAsyncTest(SpannerFixture spannerFixture) => _spannerFixture = spannerFixture;

    [Fact]
    public async Task TestUpdateInstanceAsync()
    {
        UpdateInstanceAsyncSample updateInstanceSample = new UpdateInstanceAsyncSample();

        Instance updatedInstance = await _spannerFixture.SafeAdminAsync(() =>
            updateInstanceSample.UpdateInstanceAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, Instance.Types.Edition.EnterprisePlus));

        Assert.Equal(Instance.Types.Edition.EnterprisePlus, updatedInstance.Edition);
    }
}
