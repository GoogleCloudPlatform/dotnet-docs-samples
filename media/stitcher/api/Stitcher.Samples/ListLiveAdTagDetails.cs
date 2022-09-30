﻿/*
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

// [START videostitcher_list_live_ad_tag_details]

using Google.Api.Gax;
using Google.Cloud.Video.Stitcher.V1;

public class ListLiveAdTagDetailsSample
{
    public PagedEnumerable<ListLiveAdTagDetailsResponse, LiveAdTagDetail> ListLiveAdTagDetails(
        string projectId, string regionId, string sessionId)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        ListLiveAdTagDetailsRequest request = new ListLiveAdTagDetailsRequest
        {
            ParentAsLiveSessionName = LiveSessionName.FromProjectLocationLiveSession(projectId, regionId, sessionId)
        };

        // Make the request.
        PagedEnumerable<ListLiveAdTagDetailsResponse, LiveAdTagDetail> response = client.ListLiveAdTagDetails(request);

        // Return the result.
        return response;
    }
}
// [END videostitcher_list_live_ad_tag_details]