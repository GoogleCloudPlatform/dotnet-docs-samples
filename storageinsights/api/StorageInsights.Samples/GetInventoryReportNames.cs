// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START storageinsights_get_inventory_report_names]

using Google.Cloud.StorageInsights.V1;
using System;
using System.Collections.Generic;

public class GetInventoryReportNamesSample
{
    public void GetInventoryReportNames(
        string projectId = "your-project-id",
        string bucketLocation = "us-west-1",
        string inventoryReportConfigUuid = "2b90d21c-f2f4-40b5-9519-e29a78f2b09f")
    {
        StorageInsightsClient storageInsightsClient = StorageInsightsClient.Create();
        ReportConfigName reportConfigName =
            new ReportConfigName(projectId, bucketLocation, inventoryReportConfigUuid);
        ReportConfig reportConfig = storageInsightsClient.GetReportConfig(reportConfigName);
        string extension = reportConfig.CsvOptions is null ? "csv" : "parquet";

        Console.WriteLine("You can use the Google Cloud Storage Client to download the following objects from " +
                          "Google Cloud Storage:");
        foreach (ReportDetail reportDetail in storageInsightsClient.ListReportDetails(reportConfigName))
        {
            for (long index = reportDetail.ShardsCount - 1; index >= 0; index--)
            {
                String reportName = reportDetail.ReportPathPrefix + index + "." + extension;
                Console.WriteLine(reportName);
            }
        }
    }
}
// [END storageinsights_get_inventory_report_names]
