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

// [START secretmanager_delete_secret_with_etag]

using Google.Cloud.SecretManager.V1;
using System;
using System.Threading.Tasks;

public class DeleteSecretWithEtagSample
{
    public void DeleteSecretWithEtag(
        string projectId = "my-project",
        string secretId = "my-secret")
    {
        // Create the Secret Manager client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name of the secret.
        SecretName secretName = new SecretName(projectId, secretId);

        // Get the current secret to retrieve its ETag
        Secret currentSecret = client.GetSecret(secretName);
        string etag = currentSecret.Etag;

        // Build the delete request with ETag
        DeleteSecretRequest request = new DeleteSecretRequest
        {
            Name = secretName.ToString(),
            Etag = etag
        };

        // Delete the secret with ETag
        client.DeleteSecret(request);

        Console.WriteLine($"Deleted secret: {secretName}");
    }
}
// [END secretmanager_delete_secret_with_etag]
