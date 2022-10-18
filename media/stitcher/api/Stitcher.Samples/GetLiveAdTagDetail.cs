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

// [START videostitcher_get_live_ad_tag_detail]

using Google.Cloud.Video.Stitcher.V1;

public class GetLiveAdTagDetailSample
{
    public LiveAdTagDetail GetLiveAdTagDetail(
        string projectId, string location, string sessionId, string adTagDetailId)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        GetLiveAdTagDetailRequest request = new GetLiveAdTagDetailRequest
        {
            LiveAdTagDetailName = LiveAdTagDetailName.FromProjectLocationLiveSessionLiveAdTagDetail(projectId, location, sessionId, adTagDetailId)
        };

        // Call the API.
        LiveAdTagDetail liveAdTagDetail = client.GetLiveAdTagDetail(request);

        // Return the result.
        return liveAdTagDetail;
    }
}
// [END videostitcher_get_live_ad_tag_detail]
