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

// [START livestream_create_asset]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.LiveStream.V1;
using Google.LongRunning;
using System.Threading.Tasks;

public class CreateAssetSample
{
    public async Task<Asset> CreateAssetAsync(
         string projectId, string locationId, string assetId, string assetUri)
    {
        // Create the client.
        LivestreamServiceClient client = LivestreamServiceClient.Create();

        CreateAssetRequest request = new CreateAssetRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, locationId),
            AssetId = assetId,
            Asset = new Asset
            {
                Video = new Asset.Types.VideoAsset
                {
                    Uri = assetUri
                }
            }
        };

        // Make the request.
        Operation<Asset, OperationMetadata> response = await client.CreateAssetAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<Asset, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END livestream_create_asset]