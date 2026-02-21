/*
 * Copyright 2026 Google LLC
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

// [START secretmanager_create_secret_with_cmek]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using System;

public class CreateSecretWithCmekSample
{
    public Secret CreateSecretWithCmek(
        string projectId = "my-project",
        string secretId = "my-secret",
        string kmsKeyName = "projects/my-project/locations/global/keyRings/my-keyring/cryptoKeys/my-key")
    {
        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the parent resource name.
        ProjectName projectName = new ProjectName(projectId);

        // Build the secret with automatic replication and CMEK.
        Secret secret = new Secret
        {
            Replication = new Replication
            {
                Automatic = new Replication.Types.Automatic
                {
                    CustomerManagedEncryption = new CustomerManagedEncryption
                    {
                        KmsKeyName = kmsKeyName
                    }
                }
            }
        };

        // Call the API.
        Secret createdSecret = client.CreateSecret(projectName, secretId, secret);

        // Print information about the created secret.
        Console.WriteLine($"Created secret {createdSecret.Name} with CMEK key {kmsKeyName}");
        return createdSecret;
    }
}
// [END secretmanager_create_secret_with_cmek]
