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
public class BindTagsToParameterTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly BindTagsToParameterSample _sample;

    public BindTagsToParameterTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new BindTagsToParameterSample();
    }

    [Fact]
    public void BindTagsToParameter()
    {
        var (_, tagValue) = _fixture.GetTagKeyValue();
        ParameterName parameterName = new ParameterName(_fixture.ProjectId, ParameterManagerFixture.LocationId, _fixture.RandomId());

        TagBinding result = _sample.BindTagsToParameter(projectId: _fixture.ProjectId, parameterId: parameterName.ParameterId, tagValue: tagValue);
        _fixture.ParametersToDelete.Add(parameterName);

        try
        {
            Assert.NotNull(result);
            Assert.Equal(tagValue, result.TagValue);
            Assert.Contains(parameterName.ParameterId, result.Parent);
        }
        finally
        {
            _fixture.TagBindingsClient.DeleteTagBinding(result.Name).PollUntilCompleted();
        }
    }
}
