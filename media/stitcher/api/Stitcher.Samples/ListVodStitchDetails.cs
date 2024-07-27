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

// [START videostitcher_list_vod_stitch_details]

using Google.Api.Gax;
using Google.Cloud.Video.Stitcher.V1;
using System;

public class ListVodStitchDetailsSample
{
    public PagedEnumerable<ListVodStitchDetailsResponse, VodStitchDetail> ListVodStitchDetails(
        string projectId, string regionId, string sessionId)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        ListVodStitchDetailsRequest request = new ListVodStitchDetailsRequest
        {
            ParentAsVodSessionName = VodSessionName.FromProjectLocationVodSession(projectId, regionId, sessionId)
        };

        // Make the request.
        PagedEnumerable<ListVodStitchDetailsResponse, VodStitchDetail> response = client.ListVodStitchDetails(request);
        foreach (VodStitchDetail vodStitchDetail in response)
        {
            Console.WriteLine($"{vodStitchDetail.Name}");
        }

        // Return the result.
        return response;
    }
}
// [END videostitcher_list_vod_stitch_details]