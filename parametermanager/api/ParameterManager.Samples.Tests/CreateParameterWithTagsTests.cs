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

[Collection(nameof(ParameterManagerFixture))]
public class CreateParameterWithTagsTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly CreateParameterWithTagsSample _sample;

    public CreateParameterWithTagsTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateParameterWithTagsSample();
    }

    [Fact]
    public void CreateParameterWithTags()
    {
        var (tagKey, tagValue) = _fixture.GetTagKeyValue();
        ParameterName parameterName = new ParameterName(_fixture.ProjectId, ParameterManagerFixture.LocationId, _fixture.RandomId());

        Parameter result = _sample.CreateParameterWithTags(projectId: _fixture.ProjectId, parameterId: parameterName.ParameterId, tagKey: tagKey, tagValue: tagValue);
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
