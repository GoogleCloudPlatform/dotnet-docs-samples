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
        string configId = "csharp-create-config-test111";
        Assert.NotNull(
            CreateNotificationConfigSnippets.CreateNotificationConfig(
                GetOrganizationId(), configId, GetProjectId(), GetTopic()));
        DeleteNotificationConfigSnippets.DeleteNotificationConfig(GetOrganizationId(), configId);
        Assert.True(true);
    }

    [Fact]
    public void DeleteNotificationConfig_ShouldDeleteConfig()
    {
        string configId = "csharp-delete-config-test111";
        CreateNotificationConfigSnippets.CreateNotificationConfig(
            GetOrganizationId(), configId, GetProjectId(), GetTopic());
        Assert.True(
            DeleteNotificationConfigSnippets.DeleteNotificationConfig(GetOrganizationId(), configId));
    }

    [Fact]
    public void ListNotificationConfig_ShouldReturnNonEmpty()
    {
        string configId = "csharp-list-config-test111";
        CreateNotificationConfigSnippets.CreateNotificationConfig(
            GetOrganizationId(), configId, GetProjectId(), GetTopic());
        Assert.NotNull(
            ListNotificationConfigSnippets.ListNotificationConfigs(GetOrganizationId()));
        DeleteNotificationConfigSnippets.DeleteNotificationConfig(GetOrganizationId(), configId);
    }

    [Fact]
    public void GetNotificationConfig_ShouldReturnConfig()
    {
        string configId = "csharp-get-config-test111";
        CreateNotificationConfigSnippets.CreateNotificationConfig(
            GetOrganizationId(), configId, GetProjectId(), GetTopic());
        Assert.NotNull(
            GetNotificationConfigSnippets.GetNotificationConfig(GetOrganizationId(), configId));
        DeleteNotificationConfigSnippets.DeleteNotificationConfig(GetOrganizationId(), configId);
    }

    [Fact]
    public void UpdateNotificationConfig_ShouldUpdateConfig()
    {
        string configId = "csharp-update-config-test111";
        CreateNotificationConfigSnippets.CreateNotificationConfig(
            GetOrganizationId(), configId, GetProjectId(), GetTopic());
        Assert.NotNull(
            UpdateNotificationConfigSnippets.UpdateNotificationConfig(
                GetOrganizationId(), configId, GetProjectId(), GetTopic()));
        DeleteNotificationConfigSnippets.DeleteNotificationConfig(GetOrganizationId(), configId);
    }

    private string GetOrganizationId() {
        return "1081635000895";
    }

    private string GetProjectId() {
        return "project-a-id";
    }

    private string GetTopic() {
        return "notifications-sample-topic";
    }
}
