/*
 * Copyright 2022 Google LLC
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

// [START videostitcher_list_cdn_keys]

using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Stitcher.V1;
using System;

public class ListCdnKeysSample
{
    public PagedEnumerable<ListCdnKeysResponse, CdnKey> ListCdnKeys(
        string projectId, string regionId)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        ListCdnKeysRequest request = new ListCdnKeysRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, regionId)
        };

        // Make the request.
        PagedEnumerable<ListCdnKeysResponse, CdnKey> response = client.ListCdnKeys(request);
        foreach (CdnKey cdnKey in response)
        {
            Console.WriteLine($"{cdnKey.Name}");
        }

        // Return the result.
        return response;
    }
}
// [END videostitcher_list_cdn_keys]