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

// [START parametermanager_delete_regional_param]

using Google.Cloud.ParameterManager.V1;

public class DeleteRegionalParameterSample
{
    /// <summary>
    /// This function deletes a regional parameter using the Parameter Manager SDK for GCP.
    /// </summary>
    /// <param name="projectId">The ID of the project where the parameter is located.</param>
    /// <param name="locationId">The ID of the region where the parameter is located.</param>
    /// <param name="parameterId">The ID of the parameter to be deleted.</param>
    public void DeleteRegionalParameter(
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

        // Build the resource name for the parameter in the specified regional locationId
        ParameterName parameterName = new ParameterName(projectId, locationId, parameterId);

        // Call the API to delete the parameter
        client.DeleteParameter(parameterName);

        Console.WriteLine($"Deleted regional parameter: {parameterName}");
    }
}
// [END parametermanager_delete_regional_param]
