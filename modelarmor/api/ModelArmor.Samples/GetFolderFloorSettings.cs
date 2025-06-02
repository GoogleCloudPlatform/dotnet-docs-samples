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

// [START modelarmor_get_folder_floor_settings]
using System;
using Google.Cloud.ModelArmor.V1;

public class GetFolderFloorSettings
{
    public FloorSetting GetFolderFloorSetting(string folderId)
    {
        // Initialize client.
        ModelArmorClient client = ModelArmorClient.Create();

        // Construct the resource name.
        string name = $"folders/{folderId}/locations/global/floorSetting";

        // Create the request.
        GetFloorSettingRequest request = new GetFloorSettingRequest { Name = name };

        // Make the request.
        FloorSetting response = client.GetFloorSetting(request);

        return response;
    }
}
// [END modelarmor_get_folder_floor_settings]
