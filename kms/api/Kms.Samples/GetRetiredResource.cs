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

// [START kms_get_retired_resource]
using Google.Cloud.Kms.V1;

public class GetRetiredResourceSample
{
    public RetiredResource GetRetiredResource(
        string projectId = "my-project", string locationId = "us-east1", string retiredResourceId = "my-retired-resource")
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the retired resource name.
        RetiredResourceName name = new RetiredResourceName(projectId, locationId, retiredResourceId);

        // Call the API.
        RetiredResource result = client.GetRetiredResource(name);

        // Return the result.
        return result;
    }
}
// [END kms_get_retired_resource]
