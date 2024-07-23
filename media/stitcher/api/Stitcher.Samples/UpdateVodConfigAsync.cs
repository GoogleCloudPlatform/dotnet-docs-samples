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

// [START videostitcher_update_vod_config]

using Google.Cloud.Video.Stitcher.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

public class UpdateVodConfigSample
{
    public async Task<VodConfig> UpdateVodConfigAsync(
        string projectId, string location, string vodConfigId, string sourceUri)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        UpdateVodConfigRequest request = new UpdateVodConfigRequest
        {
            VodConfig = new VodConfig
            {
                VodConfigName = VodConfigName.FromProjectLocationVodConfig(projectId, location, vodConfigId),
                SourceUri = sourceUri,
            },
            UpdateMask = new FieldMask { Paths = { "sourceUri" } }
        };

        // Make the request.
        Operation<VodConfig, OperationMetadata> response = await client.UpdateVodConfigAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<VodConfig, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END videostitcher_update_vod_config]
