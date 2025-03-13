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

// [START secretmanager_edit_regional_secret_annotations]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;


public class EditRegionalSecretAnnotationsSample
{
    public Secret EditRegionalSecretAnnotations(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret",
      string annotationKey = "my-annotation-key",
      string annotationValue = "my-annotation-value"
    )
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the resource name.
        SecretName secretName = SecretName.FromProjectLocationSecret(projectId, locationId, secretId);

        // Get the exisitng secret.
        Secret existingSecret = client.GetSecret(secretName);

        // Add the mapping to the secret's annotations
        existingSecret.Annotations[annotationKey] = annotationValue;

        // Build the field mask.
        FieldMask fieldMask = FieldMask.FromString("annotations");

        // Call the update secret API.
        Secret updatedSecret = client.UpdateSecret(existingSecret, fieldMask);

        return updatedSecret;

    }
}
// [END secretmanager_edit_regional_secret_annotations]
