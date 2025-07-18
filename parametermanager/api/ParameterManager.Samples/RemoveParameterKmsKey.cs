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

// [START parametermanager_remove_param_kms_key]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;
using Google.Protobuf.WellKnownTypes;

public class RemoveParameterKmsKeySample
{
    /// <summary>
    /// This function Removes a kms_key for parameter using the Parameter Manager SDK for GCP.
    /// </summary>
    /// <param name="projectId">The ID of the project where the parameter is to be created.</param>
    /// <param name="parameterId">The ID to assign to the new parameter. This ID must be unique within the project.</param>
    /// <returns>The Parameter object.</returns>
    public Parameter RemoveParameterKmsKey(
        string projectId,
        string parameterId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the parent resource name. 
        string name = ParameterName.Format(projectId, "global", parameterId);

        // Build the parameter.
        UpdateParameterRequest request = new UpdateParameterRequest
        {
            Parameter = new Parameter
            {
                Name = name,
            },
            UpdateMask = new FieldMask
            {
                Paths = { "kms_key" }
            }
        };

        // Call the API to create the parameter.
        Parameter updatedParameter = client.UpdateParameter(request);

        // Print the updated parameter name with kms_key.
        Console.WriteLine($"Removed kms_key for parameter {updatedParameter.Name}");

        // Return the updated parameter.
        return updatedParameter;
    }
}
// [END parametermanager_remove_param_kms_key]
