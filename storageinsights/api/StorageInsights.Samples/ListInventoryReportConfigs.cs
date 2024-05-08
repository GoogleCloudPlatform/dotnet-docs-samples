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

// [START storageinsights_list_inventory_report_configs]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.StorageInsights.V1;
using System;
using System.Collections.Generic;

public class ListInventoryReportConfigsSample
{
    public IEnumerable<ReportConfig> ListInventoryReportConfigs(
        string projectId = "your-project-id",
        string bucketLocation = "us-west1")
    {
        StorageInsightsClient storageInsightsClient = StorageInsightsClient.Create();
        Console.WriteLine($"Listing Inventory report configs in project {projectId} in location {bucketLocation}:");

        IEnumerable<ReportConfig> reportConfigs =
            storageInsightsClient.ListReportConfigs(LocationName.Format(projectId, bucketLocation));
        foreach (ReportConfig reportConfig in reportConfigs)
        {
            Console.WriteLine(reportConfig.Name);
        }

        return reportConfigs;
    }
}
// [END storageinsights_list_inventory_report_configs]
