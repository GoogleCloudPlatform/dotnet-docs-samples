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

// [START parametermanager_create_regional_param_version_with_secret]

using Google.Cloud.ParameterManager.V1;
using Google.Protobuf;
using System.Text;


public class CreateRegionalParameterVersionWithSecretSample
{
    /// <summary>
    /// This function creates a regional parameter version with a JSON payload that includes a secret reference using the Parameter Manager SDK for GCP.
    /// </summary>
    /// <param name="projectId">The ID of the project where the parameter is located.</param>
    /// <param name="locationId">The ID of the region where the parameter is located.</param>
    /// <param name="parameterId">The ID of the parameter for which the version is to be created.</param>
    /// <param name="versionId">The ID of the version to be created.</param>
    /// <param name="secretId">The ID of the secret to be referenced.</param>
    /// <returns>The created ParameterVersion object.</returns>
    public ParameterVersion CreateRegionalParameterVersionWithSecret(
        string projectId,
        string locationId,
        string parameterId,
        string versionId,
        string secretId)
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

        // Convert the JSON payload to bytes
        string payload = $"{{\"username\": \"test-user\", \"password\": \"__REF__(//secretmanager.googleapis.com/{secretId}\"}}";
        ByteString data = ByteString.CopyFrom(payload, Encoding.UTF8);

        // Build the parameter version with the JSON payload that includes a secret reference
        ParameterVersion parameterVersion = new ParameterVersion
        {
            Payload = new ParameterVersionPayload
            {
                Data = data
            }
        };

        // Call the API to create the parameter version
        ParameterVersion createdParameterVersion = client.CreateParameterVersion(parent, parameterVersion, versionId);

        // Print the created parameter version name
        Console.WriteLine($"created regional parameter version: {createdParameterVersion.Name}");

        // Return the created parameter version
        return createdParameterVersion;
    }
}
// [END parametermanager_create_regional_param_version_with_secret]
