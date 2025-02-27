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

// [START parametermanager_create_structured_param]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;

/// <summary>
/// This function creates a parameter of the specified format type using the Parameter Manager SDK for GCP.
/// </summary>
/// <param name="projectId">The ID of the project where the parameter is to be created.</param>
/// <param name="parameterId">The ID to assign to the new parameter. This ID must be unique within the project.</param>
/// <param name="format">The format type of the parameter (UNFORMATTED, YAML, JSON).</param>
/// <returns>The created Parameter object.</returns>
public class CreateStructuredParamSample
{
    public Parameter CreateStructuredParam(
        string projectId,
        string parameterId,
        ParameterFormat format)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the parent resource name. 
        LocationName parent = new LocationName(projectId, "global");

        // Build the parameter with the specified format.
        Parameter parameter = new Parameter
        {
            Format = format
        };

        // Call the API to create the parameter.
        Parameter createdParameter = client.CreateParameter(parent, parameter, parameterId);

        // Print the created parameter name.
        Console.WriteLine($"Created parameter {createdParameter.Name} with format {createdParameter.Format}");

        // Return the created parameter.
        return createdParameter;
    }
}
// [END parametermanager_create_structured_param]
