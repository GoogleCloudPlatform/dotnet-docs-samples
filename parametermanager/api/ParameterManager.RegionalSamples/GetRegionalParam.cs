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

// [START parametermanager_get_regional_param]

using Google.Cloud.ParameterManager.V1;

/// <summary>
/// This function retrieves a regional parameter using the Parameter Manager SDK for GCP.
/// </summary>
/// <param name="projectId">The ID of the project where the parameter is located.</param>
/// <param name="locationId">The ID of the region where the parameter is located.</param>
/// <param name="parameterId">The ID of the parameter to be retrieved.</param>
/// <returns>The retrieved Parameter object.</returns>
public class GetRegionalParamSample
{
    public Parameter GetRegionalParam(
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

        // Call the API to get the parameter
        Parameter parameter = client.GetParameter(parameterName);

        // Print the retrieved parameter name
        Console.WriteLine($"Found the regional parameter {parameter.Name} with format {parameter.Format}");

        // Return the retrieved parameter
        return parameter;
    }
}
// [END parametermanager_get_regional_param]
