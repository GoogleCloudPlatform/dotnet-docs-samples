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

// [START secretmanager_list_secret_versions_with_filter]

using Google.Cloud.SecretManager.V1;
using System;

public class ListSecretVersionsWithFilterSample
{
    public void ListSecretVersionsWithFilter(
        string projectId = "my-project",
        string secretId = "my-secret"
        )
    {
        string filterStr = "state:DISABLED";
        // Create the Secret Manager client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name of the parent secret.
        SecretName secretName = new SecretName(projectId, secretId);

        // Create the request with filter.
        ListSecretVersionsRequest request = new ListSecretVersionsRequest
        {
            ParentAsSecretName = secretName,
            Filter = filterStr
        };

        // List all secret versions with the provided filter.
        foreach (SecretVersion version in client.ListSecretVersions(request))
        {
            Console.WriteLine($"Found secret version: {version.Name}");
        }
    }
}
// [END secretmanager_list_secret_versions_with_filter]
