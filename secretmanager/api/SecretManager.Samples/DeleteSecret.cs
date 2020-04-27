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

// [START secretmanager_delete_secret]

using Google.Cloud.SecretManager.V1;

public class DeleteSecretSample
{
    public void DeleteSecret(
      string projectId = "my-project", string secretId = "my-secret")
    {
        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name.
        SecretName secretName = new SecretName(projectId, secretId);

        // Delete the secret.
        client.DeleteSecret(secretName);
    }
}
// [END secretmanager_delete_secret]
