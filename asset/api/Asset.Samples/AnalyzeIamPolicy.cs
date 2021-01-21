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
// [START asset_quickstart_analyze_iam_policy]

using Google.Api.Gax;
using Google.Cloud.Asset.V1;
using System.Collections.Generic;
using System.Linq;

public class AnalyzeIamPolicySample
{
    public AnalyzeIamPolicyResponse AnalyzeIamPolicy(string scope, string fullResourceName)
    {
        // Create the client.
        AssetServiceClient client = AssetServiceClient.Create();

        // Build the request.
        AnalyzeIamPolicyRequest request = new AnalyzeIamPolicyRequest
        {
            AnalysisQuery = new IamPolicyAnalysisQuery
            {
                Scope = scope,
                ResourceSelector = new IamPolicyAnalysisQuery.Types.ResourceSelector
                {
                    FullResourceName = fullResourceName,
                },
                Options = new IamPolicyAnalysisQuery.Types.Options
                {
                    ExpandGroups = true,
                    OutputGroupEdges = true,
                },
            },
        };

        // Call the API.
        AnalyzeIamPolicyResponse response = client.AnalyzeIamPolicy(request);

        // Return the result.
        return response;
    }
}
// [END asset_quickstart_analyze_iam_policy]
