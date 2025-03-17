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

// [START secretmanager_update_regional_secret_with_etag]

using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;

public class UpdateRegionalSecretWithEtagSample
{
    public Secret UpdateRegionalSecretWithEtag(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret",
      string etag = "\"1234\""
    )
    {
        // Create the Secret Manager Client with the regional endpoint.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the secret with updated fields.
        Secret secret = new Secret
        {
            SecretName = SecretName.FromProjectLocationSecret(projectId, locationId, secretId),
            Etag = etag
        };
        secret.Labels["secretmanager"] = "rocks";

        // Build the field mask.
        FieldMask fieldMask = FieldMask.FromString("labels");

        // Call the API.
        Secret updatedSecret = client.UpdateSecret(secret, fieldMask);
        return updatedSecret;
    }
}
// [END secretmanager_update_regional_secret_with_etag]
