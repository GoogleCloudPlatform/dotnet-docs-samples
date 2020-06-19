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
// [START asset_quickstart_search_all_iam_policies]

using Google.Api.Gax;
using Google.Cloud.Asset.V1;
using System.Collections.Generic;


public class SearchAllIamPoliciesSample
{
    public SearchAllIamPoliciesResponse SearchAllIamPolicies(string scope, string query = "", int pageSize = 0, string pageToken = "")
    {
        // Create the client.
        AssetServiceClient client = AssetServiceClient.Create();

        // Build the request.
        SearchAllIamPoliciesRequest request = new SearchAllIamPoliciesRequest
        {
            Scope = scope,
            Query = query,
            PageSize = pageSize,
            PageToken = pageToken,
        };

        // Call the API.
        PagedEnumerable<SearchAllIamPoliciesResponse, IamPolicySearchResult> response = client.SearchAllIamPolicies(request);

        // Return the first page.
        IEnumerator<SearchAllIamPoliciesResponse> enumerator = response.AsRawResponses().GetEnumerator();
        enumerator.MoveNext();
        return enumerator.Current;
    }
}
// [END asset_quickstart_search_all_iam_policies]
