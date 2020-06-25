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
// [START asset_quickstart_export_assets]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Asset.V1;
using System;

public class ExportAssetsSample
{
    public ExportAssetsResponse ExportAssets(string bucketName, string projectId)
    {
        string assetDumpFile = $"gs://{bucketName}/my-assets.txt";
        // Create the client
        AssetServiceClient client = AssetServiceClient.Create();
        // Build the request
        ExportAssetsRequest request = new ExportAssetsRequest
        {
            ParentAsResourceName = ProjectName.FromProject(projectId),
            OutputConfig = new OutputConfig
            {
                GcsDestination = new GcsDestination { Uri = assetDumpFile }
            }
        };
        // Start the long-running export operation
        var operation = client.ExportAssets(request);
        // Wait for it to complete (or fail)
        operation = operation.PollUntilCompleted();
        // Return the result
        return operation.Result;
    }
}
// [END asset_quickstart_export_assets]
