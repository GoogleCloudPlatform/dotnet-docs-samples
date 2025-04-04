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

// [START parametermanager_list_regional_param_versions]

using Google.Api.Gax;
using Google.Cloud.ParameterManager.V1;

public class ListRegionalParameterVersionsSample
{
    /// <summary>
    /// This function lists all versions of a regional parameter using the Parameter Manager SDK for GCP.
    /// </summary>
    /// <param name="projectId">The ID of the project where the parameter is located.</param>
    /// <param name="locationId">The ID of the region where the parameter is located.</param>
    /// <param name="parameterId"> The ID of the parameter for which the version is to be listed.</param>
    /// <returns>A list of ParameterVersion objects.</returns>
    public IEnumerable<ParameterVersion> ListRegionalParameterVersions(
        string projectId,
        string locationId,
        string parameterId)
    {
        // Define the regional endpoint
        string regionalEndpoint = $"parametermanager.{locationId}.rep.googleapis.com";

        // Create the client with the regional endpoint
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        // Build the parent resource name using ParameterName
        ParameterName parent = new ParameterName(projectId, locationId, parameterId);

        // Call the API to list the parameter versions
        PagedEnumerable<ListParameterVersionsResponse, ParameterVersion> response = client.ListParameterVersions(parent);

        // Print each parameter version name
        foreach (ParameterVersion version in response)
        {
            Console.WriteLine($"Found regional parameter version: {version.Name}");
        }

        // Return the list of parameter versions
        return response;
    }
}
// [END parametermanager_list_regional_param_versions]
