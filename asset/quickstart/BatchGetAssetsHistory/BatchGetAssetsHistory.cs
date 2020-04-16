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
// [START asset_quickstart_batch_get_assets_history]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Asset.V1;
using Google.Protobuf.WellKnownTypes;
using System;


public class BatchGetAssetsHistory
{
    public static void Main(string[] args)
    {
        // Asset names, e.g.: //storage.googleapis.com/[YOUR_BUCKET_NAME]
        string[] assetNames = { "ASSET-NAME" };
        string projectId = "YOUR-GOOGLE-PROJECT-ID";

        BatchGetAssetsHistoryRequest request = new BatchGetAssetsHistoryRequest
        {
            ParentAsResourceName = ProjectName.FromProject(projectId),
            ContentType = ContentType.Resource,
            ReadTimeWindow = new TimeWindow
            {
                StartTime = Timestamp.FromDateTime(DateTime.UtcNow)
            }
        };
        request.AssetNames.AddRange(assetNames);
        AssetServiceClient client = AssetServiceClient.Create();
        BatchGetAssetsHistoryResponse response = client.BatchGetAssetsHistory(request);
        Console.WriteLine(response);
    }
}
// [END asset_quickstart_batch_get_assets_history]
