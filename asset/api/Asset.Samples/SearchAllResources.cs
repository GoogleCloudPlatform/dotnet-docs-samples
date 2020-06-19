// Copyright(c) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.
//
// [START asset_quickstart_search_all_resources]

using Google.Api.Gax;
using Google.Cloud.Asset.V1;
using System.Collections.Generic;


public class SearchAllResourcesSample
{
    public SearchAllResourcesResponse SearchAllResources(string scope, string query = "", string[] assetTypes = null, int pageSize = 0, string pageToken = "", string orderBy = "")
    {
        // Create the client.
        AssetServiceClient client = AssetServiceClient.Create();

        // Build the request.
        SearchAllResourcesRequest request = new SearchAllResourcesRequest
        {
            Scope = scope,
            Query = query,
            PageSize = pageSize,
            PageToken = pageToken,
            OrderBy = orderBy,
        };
        request.AssetTypes.AddRange(assetTypes ?? new string[] { });

        // Call the API.
        PagedEnumerable<SearchAllResourcesResponse, ResourceSearchResult> response = client.SearchAllResources(request);

        // Return the first page.
        IEnumerator<SearchAllResourcesResponse> enumerator = response.AsRawResponses().GetEnumerator();
        enumerator.MoveNext();
        return enumerator.Current;
    }
}
// [END asset_quickstart_search_all_resources]
