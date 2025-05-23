/*
 * Copyright 2025 Google LLC
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

// [START secretmanager_update_secret_with_delayed_destroy]

using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;

public class UpdateSecretWithDelayedDestroySample
{
    public Secret UpdateSecretWithDelayedDestroy(
      string projectId = "my-project",
      string secretId = "my-secret",
      int updatedTimeToLive = 86400
    )
    {
        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the secret with updated fields.
        Secret secret = new Secret
        {
            SecretName = new SecretName(projectId, secretId),
            VersionDestroyTtl = new Duration
            {
                Seconds = updatedTimeToLive
            }
        };

        // Build the field mask.
        FieldMask fieldMask = FieldMask.FromString("version_destroy_ttl");

        // Call the API.
        Secret updatedSecret = client.UpdateSecret(secret, fieldMask);
        return updatedSecret;
    }
}
// [END secretmanager_update_secret_with_delayed_destroy]
