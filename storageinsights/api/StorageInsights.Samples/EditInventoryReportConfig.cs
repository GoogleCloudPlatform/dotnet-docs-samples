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

// [START storageinsights_edit_inventory_report_config]

using Google.Cloud.StorageInsights.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

public class EditInventoryReportConfigSample
{
    public ReportConfig EditInventoryReportConfig(
        string projectId = "your-project-id",
        string bucketLocation = "us-west-1",
        string inventoryReportConfigUuid = "2b90d21c-f2f4-40b5-9519-e29a78f2b09f")
    {
        StorageInsightsClient storageInsightsClient = StorageInsightsClient.Create();
        ReportConfigName reportConfigName =
            new ReportConfigName(projectId, bucketLocation, inventoryReportConfigUuid);
        ReportConfig reportConfig = storageInsightsClient.GetReportConfig(reportConfigName);

        // Modify any field you want to update
        reportConfig.DisplayName = "Updated display name";

        var fields = new List<string>()
        // Add any other field you want to update to this list, in snake case
        {
            "display_name"
        };

        var updatedConfig =
            storageInsightsClient.UpdateReportConfig(reportConfig, FieldMask.FromStringEnumerable<Empty>(fields));

        Console.WriteLine($"Edited report config with name {reportConfigName}");

        return updatedConfig;
    }
}
// [END storageinsights_edit_inventory_report_config]
