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

// [START livestream_list_assets]

using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.LiveStream.V1;
using System.Collections.Generic;
using System.Linq;

public class ListAssetsSample
{
    public IList<Asset> ListAssets(
        string projectId, string regionId)
    {
        // Create the client.
        LivestreamServiceClient client = LivestreamServiceClient.Create();

        ListAssetsRequest request = new ListAssetsRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, regionId)
        };

        // Make the request.
        PagedEnumerable<ListAssetsResponse, Asset> response = client.ListAssets(request);

        // The returned sequence will lazily perform RPCs as it's being iterated over.
        return response.ToList();
    }
}
// [END livestream_list_assets]