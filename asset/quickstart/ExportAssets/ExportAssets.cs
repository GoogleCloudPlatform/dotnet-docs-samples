// Copyright(c) 2018 Google LLC.
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
// [START export_assets_quickstart]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Asset.V1Beta1;
using System;


public class ExportAssets
{
    static void Main(string[] args)
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
	// string bucketName = String.Format("{0}-for-assets", projectId);
        string bucketName = String.Format("cai-prober-prod-bucket", projectId);
        string assetDumpFile = String.Format("gs://{0}/my-asset.txt", bucketName);
        AssetServiceClient client = AssetServiceClient.Create();
        ExportAssetsRequest request = new ExportAssetsRequest
        {
            ParentAsProjectName = new ProjectName(projectId),
            OutputConfig = new OutputConfig
            {
                GcsDestination = new GcsDestination { Uri = assetDumpFile }
            }
        };
        // Start the long-running export operation
        var operation = client.ExportAssets(request);
        // Wait for it to complete (or fail)
        operation = operation.PollUntilCompleted();
        // Extract the result
        ExportAssetsResponse response = operation.Result;
        Console.WriteLine(response);
    }
}
// [END export_assets_quickstart]
