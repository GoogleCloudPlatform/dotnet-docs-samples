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

// [START parametermanager_regional_quickstart]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;
using Google.Protobuf;
using System.Text;

public class RegionalQuickstartSample
{
    /// <summary>
    /// This function demonstrates how to use the Parameter Manager API with the Parameter Manager SDK for Google Cloud.
    /// It covers structured parameter creation and creation of parameter version with JSON format payload.
    /// Finally, it fetches the decoded payload and prints them.
    /// </summary>
    /// <param name="projectId">The ID of the project where the parameter is located.</param>
    /// <param name="locationId">The ID of the region where the parameter is located.</param>
    /// <param name="parameterId">The ID of the parameter for which the version is to be created.</param>
    /// <param name="versionId">The ID of the version to be created.</param>
    public void RegionalQuickstart(
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

        // Build the parent resource name for the regional locationId
        LocationName parent = new LocationName(projectId, locationId);

        // Create a parameter
        Parameter parameter = new Parameter
        {
            Format = ParameterFormat.Json
        };

        // Call the API to create the parameter.
        Parameter createdParameter = client.CreateParameter(parent, parameter, parameterId);
        Console.WriteLine($"Created regional parameter {createdParameter.Name} with format {createdParameter.Format}");

        // Build the parent resource name using ParameterName.
        ParameterName parameterName = new ParameterName(projectId, locationId, parameterId);

        // Create a parameter version with a JSON payload
        string payload = "{\"username\": \"test-user\", \"host\": \"localhost\"}";
        ByteString data = ByteString.CopyFrom(payload, Encoding.UTF8);
        ParameterVersion parameterVersion = new ParameterVersion
        {
            Payload = new ParameterVersionPayload
            {
                Data = data
            }
        };

        // Call the API to create the parameter version.
        ParameterVersion createdParameterVersion = client.CreateParameterVersion(parameterName, parameterVersion, versionId);
        Console.WriteLine($"Created regional parameter version: {createdParameterVersion.Name}");

        // Fetch the parameter version data.
        ParameterVersion getParameterVersion = client.GetParameterVersion(createdParameterVersion.Name);
        string decodedData = Encoding.UTF8.GetString(getParameterVersion.Payload.Data.ToByteArray());
        Console.WriteLine($"Payload: {decodedData}");
    }
}
// [END parametermanager_regional_quickstart]
