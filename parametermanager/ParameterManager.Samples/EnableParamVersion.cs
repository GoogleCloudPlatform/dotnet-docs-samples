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

// [START parametermanager_enable_param_version]

using Google.Cloud.ParameterManager.V1;
using Google.Protobuf.WellKnownTypes;

/// <summary>
/// This function enables a parameter version using the Parameter Manager SDK for GCP.
/// </summary>
/// <param name="projectId">The ID of the project where the parameter is located.</param>
/// <param name="parameterId">The ID of the parameter for which the version is to be enabled.</param>
/// <param name="versionId">The ID of the version to be enabled.</param>
/// <returns>The updated ParameterVersion object.</returns>
public class EnableParamVersionSample
{
    public ParameterVersion EnableParamVersion(
        string projectId,
        string parameterId,
        string versionId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the resource name for the parameter version.
        ParameterVersionName parameterVersionName = new ParameterVersionName(projectId, "global", parameterId, versionId);

        // Create the request to enable the parameter version.
        UpdateParameterVersionRequest request = new UpdateParameterVersionRequest
        {
            ParameterVersion = new ParameterVersion
            {
                Name = parameterVersionName.ToString(),
                Disabled = false
            },
            UpdateMask = new FieldMask
            {
                Paths = { "disabled" }
            }
        };

        // Call the API to update (enable) the parameter version.
        ParameterVersion updatedParameterVersion = client.UpdateParameterVersion(request);

        // Print the updated parameter version name.
        Console.WriteLine($"Enabled parameter version {versionId} for parameter {parameterId}");

        // Return the updated parameter version.
        return updatedParameterVersion;
    }
}
// [END parametermanager_enable_param_version]
