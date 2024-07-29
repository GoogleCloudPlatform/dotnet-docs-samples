/*
 * Copyright 2023 Google LLC
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

// [START videostitcher_list_live_configs]

using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Stitcher.V1;
using System;

public class ListLiveConfigsSample
{
    public PagedEnumerable<ListLiveConfigsResponse, LiveConfig> ListLiveConfigs(
        string projectId, string regionId)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        ListLiveConfigsRequest request = new ListLiveConfigsRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, regionId)
        };

        // Make the request.
        PagedEnumerable<ListLiveConfigsResponse, LiveConfig> response = client.ListLiveConfigs(request);
        foreach (LiveConfig liveConfig in response)
        {
            Console.WriteLine($"{liveConfig.Name}");
        }

        // Return the result.
        return response;
    }
}
// [END videostitcher_list_live_configs]