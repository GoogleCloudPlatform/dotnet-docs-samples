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

// [START secretmanager_edit_secret_annotations]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

public class EditSecretAnnotationsSample
{
    public Secret EditSecretAnnotations(
      string projectId = "my-project", string secretId = "my-secret", string annotationKey = "my-annotation-key", string annotationValue = "my-annotation-value")
    {
        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the secretName with the fields.
        SecretName secretName = new SecretName(projectId, secretId);

        // Get the exisitng secret.
        Secret secret = client.GetSecret(secretName);

        // Edit the Secret annotations
        secret.Annotations[annotationKey] = annotationValue;

        // Build the field mask.
        FieldMask fieldMask = FieldMask.FromString("annotations");

        // Call the API.
        Secret updatedSecret = client.UpdateSecret(secret, fieldMask);
        return updatedSecret;
    }
}
// [END secretmanager_edit_secret_annotations]
