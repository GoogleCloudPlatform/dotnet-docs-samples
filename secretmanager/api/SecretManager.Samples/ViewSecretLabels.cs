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

// [START secretmanager_view_secret_labels]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using System;

public class ViewSecretLabelsSample
{
    public Secret ViewSecretLabels(
      string projectId = "my-project", string secretId = "my-secret")
    {
        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name.
        SecretName secretName = new SecretName(projectId, secretId);

        // Fetch the secret.
        Secret secret = client.GetSecret(secretName);

        // Get the secret's labels.
        MapField<string, string> secretLabels = secret.Labels;

        // Print the labels.
        foreach (var label in secret.Labels)
        {
            Console.WriteLine($"Annotation Key: {label.Key}, Annotation Value: {label.Value}");
        }
        return secret;
    }
}
// [END secretmanager_view_secret_labels]
