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

// [START livestream_get_asset]

using Google.Cloud.Video.LiveStream.V1;

public class GetAssetSample
{
    public Asset GetAsset(
         string projectId, string locationId, string assetId)
    {
        // Create the client.
        LivestreamServiceClient client = LivestreamServiceClient.Create();

        GetAssetRequest request = new GetAssetRequest
        {
            AssetName = AssetName.FromProjectLocationAsset(projectId, locationId, assetId)
        };

        // Make the request.
        Asset response = client.GetAsset(request);
        return response;
    }
}
// [END livestream_get_asset]