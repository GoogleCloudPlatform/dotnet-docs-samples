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

using System;
using Xunit;
using Snippets;

public class SecurityCenterTests
{
    [Fact]
    public void CreateNotificationConfig_ShouldCreateConfig()
    {
        String configId = "csharp-create-config-test111";
        Assert.NotNull(
            CreateNotificationConfigSnippet.createNotificationConfig(
                GetOrganizationId(), configId, GetProjectId(), GetTopic()));
        DeleteNotificationConfigSnippets.deleteNotificationConfig(GetOrganizationId(), configId);
        Assert.True(true);
    }

    [Fact]
    public void DeleteNotificationConfig_ShouldDeleteConfig()
    {
        String configId = "csharp-delete-config-test111";
        CreateNotificationConfigSnippet.createNotificationConfig(
            GetOrganizationId(), configId, GetProjectId(), GetTopic());
        Assert.True(
            DeleteNotificationConfigSnippets.deleteNotificationConfig(GetOrganizationId(), configId));
    }

    [Fact]
    public void ListNotificationConfig_ShouldReturnNonEmpty()
    {
        String configId = "csharp-list-config-test111";
        CreateNotificationConfigSnippet.createNotificationConfig(
            GetOrganizationId(), configId, GetProjectId(), GetTopic());
        Assert.NotNull(
            ListNotificationConfigSnippets.listNotificationConfigs(GetOrganizationId()));
        DeleteNotificationConfigSnippets.deleteNotificationConfig(GetOrganizationId(), configId);
    }

    [Fact]
    public void GetNotificationConfig_ShouldReturnConfig()
    {
        String configId = "csharp-get-config-test111";
        CreateNotificationConfigSnippet.createNotificationConfig(
            GetOrganizationId(), configId, GetProjectId(), GetTopic());
        Assert.NotNull(
            GetNotificationConfigSnippets.getNotificationConfig(GetOrganizationId(), configId));
        DeleteNotificationConfigSnippets.deleteNotificationConfig(GetOrganizationId(), configId);
    }

    [Fact]
    public void UpdateNotificationConfig_ShouldUpdateConfig()
    {
        String configId = "csharp-update-config-test111";
        CreateNotificationConfigSnippet.createNotificationConfig(
            GetOrganizationId(), configId, GetProjectId(), GetTopic());
        Assert.NotNull(
            UpdateNotificationConfigSnippets.updateNotificationConfig(
                GetOrganizationId(), configId, GetProjectId(), GetTopic()));
        DeleteNotificationConfigSnippets.deleteNotificationConfig(GetOrganizationId(), configId);
    }

    private String GetOrganizationId() {
        return "1081635000895";
    }

    private String GetProjectId() {
        return "project-a-id";
    }

    private String GetTopic() {
        return "notifications-sample-topic";
    }
}
