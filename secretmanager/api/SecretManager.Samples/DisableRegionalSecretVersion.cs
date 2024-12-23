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

// [START secretmanager_disable_regional_secret_version]

using Google.Cloud.SecretManager.V1;

public class DisableRegionalSecretVersionSample
{
    public SecretVersion DisableRegionalSecretVersion(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret",
      string secretVersionId = "123"
    )
    {
        // Create the Secret Manager Client with the regional endpoint.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the resource name.
        SecretVersionName secretVersionName = SecretVersionName.FromProjectLocationSecretSecretVersion(projectId, locationId, secretId, secretVersionId);

        // Call the API.
        SecretVersion version = client.DisableSecretVersion(secretVersionName);
        return version;
    }
}
// [END secretmanager_disable_regional_secret_version]
