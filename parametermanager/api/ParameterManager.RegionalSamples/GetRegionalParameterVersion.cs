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

// [START parametermanager_get_regional_param_version]

using Google.Cloud.ParameterManager.V1;
using System.Text;

public class GetRegionalParameterVersionSample
{
    /// <summary>
    /// This function retrieves a regional parameter version using the Parameter Manager SDK for GCP.
    /// </summary>
    /// <param name="projectId">The ID of the project where the parameter is located.</param>
    /// <param name="locationId">The ID of the region where the parameter is located.</param>
    /// <param name="parameterId">The ID of the parameter for which the version is to be retrieved.</param>
    /// <param name="versionId">The ID of the version to be retrieved.</param>
    /// <returns>The retrieved ParameterVersion object.</returns>
    public ParameterVersion GetRegionalParameterVersion(
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

        // Call the API to get the parameter version
        ParameterVersion parameterVersion = client.GetParameterVersion(parameterVersionName);



        // Print the retrieved parameter version name
        Console.WriteLine($"Found regional parameter version {parameterVersion.Name} with state {(parameterVersion.Disabled ? "disabled" : "enabled")}");

        if (!parameterVersion.Disabled)
        {
            Console.WriteLine($"Payload: {Encoding.UTF8.GetString(parameterVersion.Payload.Data.ToByteArray())}");
        }

        // Return the retrieved parameter version
        return parameterVersion;
    }
}
// [END parametermanager_get_regional_param_version]
