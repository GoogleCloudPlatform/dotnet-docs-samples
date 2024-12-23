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

// [START secretmanager_list_regional_secret_versions]

using Google.Cloud.SecretManager.V1;
using System;
<<<<<<< HEAD
=======
using System.Collections.Generic;
using System.Linq;
>>>>>>> 95d07aff (chore: Add SecretManager service regional code samples)

public class ListRegionalSecretVersionsSample
{
    public List<SecretVersion> ListRegionalSecretVersions(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret"
    )
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the resource name.
        SecretName secretName = SecretName.FromProjectLocationSecret(projectId, locationId, secretId);

        // Call the API.
        List<SecretVersion> secretVersions = client.ListSecretVersions(secretName).ToList();

        // Traverse the secret versions list.
        foreach (SecretVersion secretVersion in secretVersions)
        {
            Console.WriteLine($"Got regional secret version : {secretVersion.Name}");
        }

        return secretVersions;
    }
}
// [END secretmanager_list_regional_secret_versions]
