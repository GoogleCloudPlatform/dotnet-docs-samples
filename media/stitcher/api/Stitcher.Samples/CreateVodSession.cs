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

// [START videostitcher_create_vod_session]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Stitcher.V1;

public class CreateVodSessionSample
{
    public VodSession CreateVodSession(
        string projectId, string location, string sourceUri, string adTagUri)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        CreateVodSessionRequest request = new CreateVodSessionRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, location),
            VodSession = new VodSession
            {
                SourceUri = sourceUri,
                AdTagUri = adTagUri
            }
        };

        // Call the API.
        VodSession session = client.CreateVodSession(request);

        // Return the result.
        return session;
    }
}
// [END videostitcher_create_vod_session]
