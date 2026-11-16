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
public class GetRegionalParameterTagsTests
{
    private readonly ParameterManagerRegionalFixture _fixture;
    private readonly GetRegionalParameterTagsSample _sample;

    public GetRegionalParameterTagsTests(ParameterManagerRegionalFixture fixture)
    {
        _fixture = fixture;
        _sample = new GetRegionalParameterTagsSample();
    }

    [Fact]
    public void GetRegionalParameterTags()
    {
        var (_, tagValue) = _fixture.GetTagKeyValue();
        Parameter parameter = _fixture.CreateParameter(_fixture.RandomId(), ParameterFormat.Unformatted);
        TagBinding tagBinding = _fixture.TagBindingsClient.CreateTagBinding(new TagBinding
        {
            Parent = $"//parametermanager.googleapis.com/{parameter.Name}",
            TagValue = tagValue
        }).PollUntilCompleted().Result;

        try
        {
            IEnumerable<TagBinding> result = _sample.GetRegionalParameterTags(projectId: _fixture.ProjectId, locationId: ParameterManagerRegionalFixture.LocationId, parameterId: parameter.ParameterName.ParameterId);

            Assert.NotNull(result);
            Assert.Contains(tagValue, result.Select(b => b.TagValue));
        }
        finally
        {
            _fixture.TagBindingsClient.DeleteTagBinding(tagBinding.Name).PollUntilCompleted();
        }
    }
}
