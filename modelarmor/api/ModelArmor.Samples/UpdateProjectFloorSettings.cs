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

// [START modelarmor_update_project_floor_settings]
using System;
using Google.Cloud.ModelArmor.V1;

public class UpdateProjectFloorSettings
{
    public FloorSetting UpdateProjectFloorSetting(string projectId)
    {
        // Create the client
        ModelArmorClient client = ModelArmorClient.Create();

        // Construct the floor settings name
        string floorSettingsName = $"projects/{projectId}/locations/global/floorSetting";

        // Build the floor settings with your preferred filters
        // For more details on filters, please refer to the following doc:
        // https://cloud.google.com/security-command-center/docs/key-concepts-model-armor#ma-filters
        var raiFilter = new RaiFilterSettings.Types.RaiFilter
        {
            FilterType = RaiFilterType.HateSpeech,
            ConfidenceLevel = DetectionConfidenceLevel.High,
        };

        var raiFilterSettings = new RaiFilterSettings();
        raiFilterSettings.RaiFilters.Add(raiFilter);

        var filterConfig = new FilterConfig { RaiSettings = raiFilterSettings };

        var floorSetting = new FloorSetting
        {
            Name = floorSettingsName,
            FilterConfig = filterConfig,
            EnableFloorSettingEnforcement = true,
        };

        // Create the update request
        var updateRequest = new UpdateFloorSettingRequest { FloorSetting = floorSetting };

        // Update the floor settings
        FloorSetting response = client.UpdateFloorSetting(updateRequest);

        Console.WriteLine($"Floor setting updated: {response.Name}");

        return response;
    }
}
// [END modelarmor_update_project_floor_settings]
