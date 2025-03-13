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

// [START secretmanager_destroy_regional_secret_version_with_etag]

using Google.Cloud.SecretManager.V1;

public class DestroyRegionalSecretVersionWithEtagSample
{
    public SecretVersion DestroyRegionalSecretVersionWithEtag(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret",
      string secretVersionId = "5",
      string etag = "\"1234\""
    )
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the resource name.
        DestroySecretVersionRequest request = new DestroySecretVersionRequest
        {
            SecretVersionName = SecretVersionName.FromProjectLocationSecretSecretVersion(
              projectId, locationId, secretId, secretVersionId
            ),
            Etag = etag
        };

        // Call the API.
        SecretVersion version = client.DestroySecretVersion(request);
        return version;
    }
}
// [END secretmanager_destroy_regional_secret_version_with_etag]
