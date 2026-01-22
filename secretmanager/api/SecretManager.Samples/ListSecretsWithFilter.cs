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

// [START secretmanager_list_secrets_with_filter]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using System;

public class ListSecretsWithFilterSample
{
    public void ListSecretsWithFilter(string projectId = "my-project")
    {
        string filterStr = "labels.my-label-key=my-label-value";
        // Create the Secret Manager client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name of the parent project.
        ProjectName projectName = new ProjectName(projectId);
        ListSecretsRequest request = new ListSecretsRequest
        {
            ParentAsProjectName = projectName,
            Filter = filterStr
        };


        // List all secrets with the provided filter.
        foreach (Secret secret in client.ListSecrets(request))
        {
            Console.WriteLine($"Found secret: {secret.Name}");
        }
    }
}
// [END secretmanager_list_secrets_with_filter]
