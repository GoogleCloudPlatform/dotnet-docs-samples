/*
 * Copyright 2020 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Xunit;

[Collection(nameof(ConfigFixture))]
public class SecurityCenterTests
{
    private readonly ConfigFixture _fixture;
    public SecurityCenterTests(ConfigFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CreateNotificationConfig_ShouldCreateConfig()
    {
        string configId = _fixture.RandomId();

        Assert.NotNull(CreateNotificationConfigSnippets.CreateNotificationConfig(_fixture.OrganizationId, configId, _fixture.ProjectId, _fixture.Topic));

        _fixture.MarkForDeletion(configId);
    }

    [Fact]
    public void DeleteNotificationConfig_ShouldDeleteConfig()
    {
        string configId = _fixture.RandomId();

        CreateNotificationConfigSnippets.CreateNotificationConfig(_fixture.OrganizationId, configId, _fixture.ProjectId, _fixture.Topic);
        _fixture.MarkForDeletion(configId);

        Assert.True(DeleteNotificationConfigSnippets.DeleteNotificationConfig(_fixture.OrganizationId, configId));
    }

    [Fact]
    public void ListNotificationConfig_ShouldReturnNonEmpty()
    {
        Assert.NotEmpty(ListNotificationConfigSnippets.ListNotificationConfigs(_fixture.OrganizationId));
    }

    [Fact]
    public void GetNotificationConfig_ShouldReturnConfig()
    {
        Assert.NotNull(GetNotificationConfigSnippets.GetNotificationConfig(_fixture.OrganizationId, _fixture.DefaultNotificationConfigId));
    }

    [Fact]
    public void UpdateNotificationConfig_ShouldUpdateConfig()
    {
        Assert.NotNull(UpdateNotificationConfigSnippets.UpdateNotificationConfig(_fixture.OrganizationId, _fixture.DefaultNotificationConfigId, _fixture.ProjectId, _fixture.Topic));
    }
}
