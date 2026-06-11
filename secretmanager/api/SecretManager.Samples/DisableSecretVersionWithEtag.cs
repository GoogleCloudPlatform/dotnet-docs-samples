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

// [START secretmanager_disable_secret_version_with_etag]

using Google.Cloud.SecretManager.V1;
using System;
using System.Threading.Tasks;

public class DisableSecretVersionWithEtagSample
{
    // Synchronous version of the method
    public SecretVersion DisableSecretVersionWithEtag(
        string projectId = "my-project",
        string secretId = "my-secret",
        string versionId = "1")
    {
        // Create the Secret Manager client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name of the secret version
        SecretVersionName secretVersionName = new SecretVersionName(projectId, secretId, versionId);

        // Get the current version to retrieve its ETag
        SecretVersion currentVersion = client.GetSecretVersion(secretVersionName);
        string etag = currentVersion.Etag;

        // Build the disable request with ETag
        DisableSecretVersionRequest request = new DisableSecretVersionRequest
        {
            Name = secretVersionName.ToString(),
            Etag = etag
        };

        // Disable the secret version
        SecretVersion disabledVersion = client.DisableSecretVersion(request);

        Console.WriteLine($"Disabled secret version: {disabledVersion.Name}");
        return disabledVersion;
    }
}
// [END secretmanager_disable_secret_version_with_etag]
