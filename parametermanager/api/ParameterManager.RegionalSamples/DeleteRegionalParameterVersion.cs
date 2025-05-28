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

// [START parametermanager_delete_regional_param_version]

using Google.Cloud.ParameterManager.V1;

public class DeleteRegionalParameterVersionSample
{
    /// <summary>
    /// This function deletes a regional parameter version using the Parameter Manager SDK for GCP.
    /// </summary>
    /// <param name="projectId">The ID of the project where the parameter is located.</param>
    /// <param name="locationId">The ID of the region where the parameter is located.</param>
    /// <param name="parameterId">The ID of the parameter for which the version is to be deleted.</param>
    /// <param name="versionId">The ID of the version to be deleted.</param>
    public void DeleteRegionalParameterVersion(
        string projectId,
        string locationId,
        string parameterId,
        string versionId)
    {
        // Define the regional endpoint
        string regionalEndpoint = $"parametermanager.{locationId}.rep.googleapis.com";

        // Create the client with the regional endpoint
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        // Build the resource name for the parameter version in the specified regional locationId
        ParameterVersionName parameterVersionName = new ParameterVersionName(projectId, locationId, parameterId, versionId);

        // Call the API to delete the parameter version
        client.DeleteParameterVersion(parameterVersionName);

        Console.WriteLine($"Deleted regional parameter version: {parameterVersionName}");
    }
}
// [END parametermanager_delete_regional_param_version]
