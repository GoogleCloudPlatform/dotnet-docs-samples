/*
 * Copyright 2026 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Cloud.ParameterManager.V1;
using Google.Cloud.ResourceManager.V3;

[Collection(nameof(ParameterManagerRegionalFixture))]
public class CreateRegionalParameterWithTagsTests
{
    private readonly ParameterManagerRegionalFixture _fixture;
    private readonly CreateRegionalParameterWithTagsSample _sample;

    public CreateRegionalParameterWithTagsTests(ParameterManagerRegionalFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateRegionalParameterWithTagsSample();
    }

    [Fact]
    public void CreateRegionalParameterWithTags()
    {
        var (tagKey, tagValue) = _fixture.GetTagKeyValue();
        ParameterName parameterName = new ParameterName(_fixture.ProjectId, ParameterManagerRegionalFixture.LocationId, _fixture.RandomId());

        Parameter result = _sample.CreateRegionalParameterWithTags(projectId: _fixture.ProjectId, locationId: ParameterManagerRegionalFixture.LocationId, parameterId: parameterName.ParameterId, tagKey: tagKey, tagValue: tagValue);
        _fixture.ParametersToDelete.Add(parameterName);

        Assert.NotNull(result);
        Assert.Equal(parameterName.ParameterId, result.ParameterName.ParameterId);

        // Confirm the tag through Resource Manager, since tags are input-only.
        ListTagBindingsRequest request = new ListTagBindingsRequest
        {
            Parent = $"//parametermanager.googleapis.com/{result.Name}"
        };
        List<string> tagValues = _fixture.TagBindingsClient.ListTagBindings(request).Select(b => b.TagValue).ToList();
        Assert.Contains(tagValue, tagValues);
    }
}
