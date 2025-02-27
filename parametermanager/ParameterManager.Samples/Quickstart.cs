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

// [START parametermanager_quickstart]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;
using Google.Protobuf;
using System.Text;

/// <summary>
/// This function demonstrates the quickstart for using the parameter manager using the Parameter Manager SDK for GCP.
/// It covers structured parameter creation, creation of parameter version with JSON format payload which has secret reference in it.
/// Finally, it fetches the simple and decoded payload and prints them.
/// </summary>
/// <param name="projectId">The ID of the project where the parameter is located.</param>
/// <param name="parameterId">The ID of the parameter for which the version is to be created.</param>
/// <param name="versionId">The ID of the version to be created.</param>
public class QuickstartSample
{
    public void Quickstart(
        string projectId,
        string parameterId,
        string versionId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the parent resource name. 
        LocationName parent = new LocationName(projectId, "global");

        // Create a structured parameter.
        Parameter parameter = new Parameter
        {
            Format = ParameterFormat.Json
        };

        // Call the API to create the parameter.
        Parameter createdParameter = client.CreateParameter(parent, parameter, parameterId);
        Console.WriteLine($"Created parameter {createdParameter.Name} with format {createdParameter.Format}");

        // Build the parent resource name using ParameterName.
        ParameterName parameterName = new ParameterName(projectId, "global", parameterId);

        // Define the JSON payload
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
        ParameterVersion createdParameterVersion = client.CreateParameterVersion(parameterName.ToString(), parameterVersion, versionId);
        Console.WriteLine($"Created parameter version: {createdParameterVersion.Name}");

        // Fetch the parameter version data.
        ParameterVersion getParameterVersion = client.GetParameterVersion(createdParameterVersion.Name);
        string decodedData = Encoding.UTF8.GetString(getParameterVersion.Payload.Data.ToByteArray());
        Console.WriteLine($"Payload: {decodedData}");
    }
}
// [END parametermanager_quickstart]
