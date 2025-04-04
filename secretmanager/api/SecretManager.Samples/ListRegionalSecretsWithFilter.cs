/*
 * Copyright 2024 Google LLC
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

// [START secretmanager_list_regional_secrets_with_filter]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using System;
using System.Collections.Generic;
using System.Linq;

public class ListRegionalSecretsWithFilterSample
{
    public List<Secret> ListRegionalSecretsWithFilter(
      string projectId = "my-project", string locationId = "my-location", string filter = "create_time>2024-01-01T00:00:00Z"
    )
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the parent resource name.
        LocationName locationName = new LocationName(projectId, locationId);

        ListSecretsRequest request = new ListSecretsRequest
        {
            Parent = locationName.ToString(),
            Filter = filter,
        };

        // Call the API.
        List<Secret> secrets = client.ListSecrets(request).ToList();

        // Traverse the secret list.
        foreach (Secret secret in secrets)
        {
            Console.WriteLine($"Got regional secret : {secret.Name}");
        }

        return secrets;
    }
}
// [END secretmanager_list_regional_secrets_with_filter]
