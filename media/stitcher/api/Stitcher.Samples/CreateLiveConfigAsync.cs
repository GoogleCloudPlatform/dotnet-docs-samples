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

// [START videostitcher_create_live_config]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Stitcher.V1;
using Google.LongRunning;
using System.Threading.Tasks;

public class CreateLiveConfigSample
{
    public async Task<LiveConfig> CreateLiveConfigAsync(
        string projectId, string location, string liveConfigId, string sourceUri, string adTagUri, string slateId)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        CreateLiveConfigRequest request = new CreateLiveConfigRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, location),
            LiveConfigId = liveConfigId,
            LiveConfig = new LiveConfig
            {
                SourceUri = sourceUri,
                AdTagUri = adTagUri,
                DefaultSlate = SlateName.FormatProjectLocationSlate(projectId, location, slateId),
                AdTracking = AdTracking.Server,
                StitchingPolicy = LiveConfig.Types.StitchingPolicy.CutCurrent
            }
        };

        // Make the request.
        Operation<LiveConfig, OperationMetadata> response = await client.CreateLiveConfigAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<LiveConfig, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END videostitcher_create_live_config]
