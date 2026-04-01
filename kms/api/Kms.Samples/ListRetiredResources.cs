// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START kms_list_retired_resources]
using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Kms.V1;
using System.Collections.Generic;

public class ListRetiredResourcesSample
{
    public IEnumerable<RetiredResource> ListRetiredResources(
        string projectId = "my-project", string locationId = "us-east1")
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the location name.
        LocationName locationName = new LocationName(projectId, locationId);

        // Call the API.
        PagedEnumerable<ListRetiredResourcesResponse, RetiredResource> response = client.ListRetiredResources(locationName);

        // Return the result.
        return response;
    }
}
// [END kms_list_retired_resources]
