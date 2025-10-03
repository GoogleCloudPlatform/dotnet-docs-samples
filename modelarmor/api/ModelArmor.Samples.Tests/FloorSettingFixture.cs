/*
 * Copyright 2025 Google LLC
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

using System;
using Google.Cloud.ModelArmor.V1;
using Xunit;

[CollectionDefinition("FloorSettingCollection")]
public class FloorSettingFixture : IDisposable, ICollectionFixture<FloorSettingFixture>
{
    private const string EnvProjectId = "GOOGLE_PROJECT_ID";

    public ModelArmorClient Client { get; }
    public string ProjectId { get; }
    public string FolderId { get; }
    public string OrganizationId { get; }

    public FloorSettingFixture()
    {
        ProjectId = GetRequiredEnvVar(EnvProjectId);
        FolderId = GetRequiredEnvVar("MA_FOLDER_ID");
        OrganizationId = GetRequiredEnvVar("MA_ORG_ID");

        // Create the Model Armor client.
        Client = ModelArmorClient.Create();

        ResetFloorSettings();
    }

    public string GetRequiredEnvVar(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
        {
            throw new Exception($"Missing {name} environment variable");
        }
        return value;
    }

    public void Dispose()
    {
        ResetFloorSettings();
    }

    // Reset floor settings to default values for project, folder, and organization
    private void ResetFloorSettings()
    {
        // Reset project floor settings if project ID is available
        if (!string.IsNullOrEmpty(ProjectId))
        {
            Console.WriteLine($"Resetting project floor settings for project ID: {ProjectId}");
            ResetProjectFloorSettings(ProjectId);
        }

        // Reset folder floor settings if folder ID is available
        string folderId = FolderId;
        if (!string.IsNullOrEmpty(folderId))
        {
            Console.WriteLine($"Resetting folder floor settings for folder ID: {folderId}");
            ResetFolderFloorSettings(folderId);
        }

        // Reset organization floor settings if organization ID is available
        string organizationId = OrganizationId;
        if (!string.IsNullOrEmpty(organizationId))
        {
            Console.WriteLine(
                $"Resetting organization floor settings for organization ID: {organizationId}"
            );
            ResetOrganizationFloorSettings(organizationId);
        }
    }

    // Reset project floor settings to default
    public void ResetProjectFloorSettings(string projectId)
    {
        try
        {
            // Create default floor setting with empty RAI filters and enforcement disabled
            FloorSetting defaultFloorSetting = new FloorSetting
            {
                Name = $"projects/{projectId}/locations/global/floorSetting",
                FilterConfig = new FilterConfig
                {
                    RaiSettings = new RaiFilterSettings { RaiFilters = { } },
                },
                EnableFloorSettingEnforcement = false,
            };

            // Update the floor setting to reset it
            Client.UpdateFloorSetting(
                new UpdateFloorSettingRequest { FloorSetting = defaultFloorSetting }
            );
            Console.WriteLine($"Project floor settings reset for project ID: {projectId}");
        }
        catch (Exception ex)
        {
            // Log but don't throw to avoid breaking test cleanup
            Console.WriteLine($"Error resetting project floor settings: {ex.Message}");
        }
    }

    // Reset folder floor settings to default
    public void ResetFolderFloorSettings(string folderId)
    {
        try
        {
            // Create default floor setting with empty RAI filters and enforcement disabled
            FloorSetting defaultFloorSetting = new FloorSetting
            {
                Name = $"folders/{folderId}/locations/global/floorSetting",
                FilterConfig = new FilterConfig
                {
                    RaiSettings = new RaiFilterSettings { RaiFilters = { } },
                },
                EnableFloorSettingEnforcement = false,
            };

            // Update the floor setting to reset it
            Client.UpdateFloorSetting(
                new UpdateFloorSettingRequest { FloorSetting = defaultFloorSetting }
            );
            Console.WriteLine($"Folder floor settings reset for folder ID: {folderId}");
        }
        catch (Exception ex)
        {
            // Log but don't throw to avoid breaking test cleanup
            Console.WriteLine($"Error resetting folder floor settings: {ex.Message}");
        }
    }

    // Reset organization floor settings to default
    public void ResetOrganizationFloorSettings(string organizationId)
    {
        try
        {
            // Create default floor setting with empty RAI filters and enforcement disabled
            FloorSetting defaultFloorSetting = new FloorSetting
            {
                Name = $"organizations/{organizationId}/locations/global/floorSetting",
                FilterConfig = new FilterConfig
                {
                    RaiSettings = new RaiFilterSettings { RaiFilters = { } },
                },
                EnableFloorSettingEnforcement = false,
            };

            // Update the floor setting to reset it
            Client.UpdateFloorSetting(
                new UpdateFloorSettingRequest { FloorSetting = defaultFloorSetting }
            );
            Console.WriteLine(
                $"Organization floor settings reset for organization ID: {organizationId}"
            );
        }
        catch (Exception ex)
        {
            // Log but don't throw to avoid breaking test cleanup
            Console.WriteLine($"Error resetting organization floor settings: {ex.Message}");
        }
    }
}
