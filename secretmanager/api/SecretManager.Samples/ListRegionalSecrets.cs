/*
 * Copyright 2020 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START secretmanager_list_regional_secrets]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;

public class ListRegionalSecretsSample
{
    public void ListRegionalSecrets(string projectId = "my-project", string locationId = "my-location")
    {
        // Create the Secret Manager Client with the regional endpoint.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the parent resource name.
        LocationName locationName = new LocationName(projectId, locationId);

        // Call the API.
        foreach (Secret secret in client.ListSecrets(locationName))
        {
            // ...
        }
    }
}
// [END secretmanager_list_regional_secrets]
