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

// [START parametermanager_create_param_with_kms_key]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;

public class CreateParameterWithKmsKeySample
{
    /// <summary>
    /// This function creates a parameter with kms_key using the Parameter Manager SDK for GCP.
    /// </summary>
    /// <param name="projectId">The ID of the project where the parameter is to be created.</param>
    /// <param name="parameterId">The ID to assign to the new parameter. This ID must be unique within the project.</param>
    /// <param name="kmsKey">The ID of the KMS key to be used for encryption.
    /// (e.g. "projects/my-project/locations/global/keyRings/my-key-ring/cryptoKeys/my-encryption-key")</param>
    /// <returns>The created Parameter object.</returns>
    public Parameter CreateParameterWithKmsKey(
        string projectId,
        string parameterId,
        string kmsKey)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the parent resource name. 
        LocationName parent = new LocationName(projectId, "global");

        // Build the parameter.
        Parameter parameter = new Parameter
        {
            KmsKey = kmsKey
        };

        // Call the API to create the parameter.
        Parameter createdParameter = client.CreateParameter(parent, parameter, parameterId);

        // Print the created parameter name with kms_key.
        Console.WriteLine($"Created parameter {createdParameter.Name} with kms_key {createdParameter.KmsKey}");

        // Return the created parameter.
        return createdParameter;
    }
}
// [END parametermanager_create_param_with_kms_key]
