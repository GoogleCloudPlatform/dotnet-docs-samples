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

// [START secretmanager_delete_secret_label]

using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class DeleteSecretLabelSample
{
    public void DeleteSecretLabel(
      string projectId = "my-project", string secretId = "my-secret")
    {
        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name of the secret.
        SecretName secretName = new SecretName(projectId, secretId);

        // Get the secret.
        Secret secret = client.GetSecret(secretName);

        // Clear all labels
        secret.Labels.Clear();

        // Build the field mask.
        FieldMask updateMask = FieldMask.FromString("labels");

        // Update the secret.
        Secret updatedSecret = client.UpdateSecret(secret, updateMask);

        // Print the new secret name.
        Console.WriteLine($"Updated secret: {updatedSecret.Name}");
    }
}
// [END secretmanager_delete_secret_label]
