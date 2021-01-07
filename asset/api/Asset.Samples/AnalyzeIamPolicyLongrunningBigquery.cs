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
// [START asset_quickstart_analyze_iam_policy_longrunning_bigquery]

using Google.Api.Gax;
using Google.Cloud.Asset.V1;
using System.Collections.Generic;
using System.Linq;

public class AnalyzeIamPolicyLongrunningBigquerySample
{
    public AnalyzeIamPolicyLongrunningRequest AnalyzeIamPolicyLongrunning(
      string scope, string fullResourceName, string dataset, string tablePrefix)
    {
        // Create the client.
        AssetServiceClient client = AssetServiceClient.Create();

        // Build the request.
        AnalyzeIamPolicyLongrunningRequest request = new AnalyzeIamPolicyLongrunningRequest
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
            OutputConfig = new IamPolicyAnalysisOutputConfig
            {
                BigqueryDestination = new IamPolicyAnalysisOutputConfig.Types.BigQueryDestination
                {
                    Dataset = dataset,
                    TablePrefix = tablePrefix,
                },
            },
        };

        // Start the analyze long-running operation
        var operation = client.AnalyzeIamPolicyLongrunning(request);
        // Wait for it to complete
        operation = operation.PollUntilCompleted();
        // Return the metadata(request)
        return operation.Metadata;
    }
}
// [END asset_quickstart_analyze_iam_policy_longrunning_bigquery]
