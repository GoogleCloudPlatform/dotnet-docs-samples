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

// [START parametermanager_list_params]

using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;

public class ListParametersSample
{
    /// <summary>
    /// This function lists parameter using the Parameter Manager SDK for GCP.
    /// </summary>
    /// <param name="projectId">The ID of the project where the parameter is located.</param>
    /// <returns>A list of Parameter objects.</returns>
    public IEnumerable<Parameter> ListParameters(string projectId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the parent resource name.
        LocationName parent = new LocationName(projectId, "global");

        // Call the API to list the parameters.
        PagedEnumerable<ListParametersResponse, Parameter> response = client.ListParameters(parent);

        // Print each parameter name.
        foreach (Parameter parameter in response)
        {
            Console.WriteLine($"Found parameter {parameter.Name} with format {parameter.Format}");
        }

        // Return the list of parameters.
        return response;
    }
}
// [END parametermanager_list_params]
