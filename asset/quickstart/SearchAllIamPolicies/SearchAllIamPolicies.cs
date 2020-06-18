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
using System;
using System.Collections.Generic;


public class SearchAllIamPolicies
{
    public static void Main(string[] args)
    {
        string scope = "SCOPE-OF-THE-SEARCH";
        string query = "QUERY-STATEMENT";
        int pageSize = 0;
        string pageToken = "";

        AssetServiceClient client = AssetServiceClient.Create();
        SearchAllIamPoliciesRequest request = new SearchAllIamPoliciesRequest
        {
            Scope = scope,
            Query = query,
            PageSize = pageSize,
            PageToken = pageToken,
        };
        PagedEnumerable<SearchAllIamPoliciesResponse, IamPolicySearchResult> response = client.SearchAllIamPolicies(request);

        // Extract the first page
        IEnumerator<SearchAllIamPoliciesResponse> enumerator = response.AsRawResponses().GetEnumerator();
        enumerator.MoveNext();
        SearchAllIamPoliciesResponse page = enumerator.Current;

        Console.WriteLine(page.Results);
    }
}
// [END asset_quickstart_search_all_iam_policies]
