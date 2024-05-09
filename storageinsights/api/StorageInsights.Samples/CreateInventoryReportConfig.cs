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

// [START storageinsights_create_inventory_report_config]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.StorageInsights.V1;
using Google.Type;
using System;
using DateTime = System.DateTime;

public class CreateInventoryReportConfigSample
{
    public ReportConfig CreateInventoryReportConfig(
        string projectId = "your-project-id",
        string bucketLocation = "us-west-1",
        string sourceBucket = "your-gcs-source-bucket",
        string destinationBucket = "your-gcs-destination-bucket")
    {
        ReportConfig reportConfig = new ReportConfig
        {
            DisplayName = "Example inventory report configuration",
            FrequencyOptions = new FrequencyOptions
            {
                Frequency = FrequencyOptions.Types.Frequency.Weekly,
                StartDate = Date.FromDateTime(DateTime.UtcNow.AddDays(1)),
                EndDate = Date.FromDateTime(DateTime.UtcNow.AddDays(2))
            },
            CsvOptions = new CSVOptions { RecordSeparator = "\n", Delimiter = ",", HeaderRequired = true },
            ObjectMetadataReportOptions = new ObjectMetadataReportOptions
            {
                MetadataFields = { "project", "name", "bucket" },
                StorageFilters = new CloudStorageFilters { Bucket = sourceBucket },
                StorageDestinationOptions = new CloudStorageDestinationOptions { Bucket = destinationBucket }
            },
        };

        CreateReportConfigRequest request = new CreateReportConfigRequest
        {
            Parent = LocationName.Format(projectId, bucketLocation), ReportConfig = reportConfig
        };

        StorageInsightsClient client = StorageInsightsClient.Create();

        ReportConfig response = client.CreateReportConfig(request);

        Console.WriteLine($"Created a new Inventory Report Config with name {response.Name}");

        return response;
    }
}
// [END storageinsights_create_inventory_report_config]
