/*
 * Copyright 2024 Google LLC
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

// [START videostitcher_create_vod_config]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Stitcher.V1;
using Google.LongRunning;
using System.Threading.Tasks;

public class CreateVodConfigSample
{
    public async Task<VodConfig> CreateVodConfigAsync(
        string projectId, string location, string vodConfigId, string sourceUri, string adTagUri)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        CreateVodConfigRequest request = new CreateVodConfigRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, location),
            VodConfigId = vodConfigId,
            VodConfig = new VodConfig
            {
                SourceUri = sourceUri,
                AdTagUri = adTagUri
            }
        };

        // Make the request.
        Operation<VodConfig, OperationMetadata> response = await client.CreateVodConfigAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<VodConfig, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END videostitcher_create_vod_config]
