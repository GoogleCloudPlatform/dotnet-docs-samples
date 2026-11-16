/*
 * Copyright 2026 Google LLC
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

// [START parametermanager_get_param_version_verify_checksum]

using Force.Crc32;
using Google.Cloud.ParameterManager.V1;

public class GetParameterVersionVerifyChecksumSample
{
    /// <summary>
    /// Gets a parameter version and verifies its payload checksum.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="parameterId">The ID of the parameter.</param>
    /// <param name="versionId">The ID of the parameter version.</param>
    public ParameterVersion GetParameterVersionVerifyChecksum(
        string projectId,
        string parameterId,
        string versionId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the request with the full view to include the payload and checksum.
        GetParameterVersionRequest request = new GetParameterVersionRequest
        {
            ParameterVersionName = new ParameterVersionName(projectId, "global", parameterId, versionId),
            View = View.Full
        };

        // Call the API.
        ParameterVersion parameterVersion = client.GetParameterVersion(request);

        // Verify the payload checksum.
        uint computedCrc32c = Crc32CAlgorithm.Compute(parameterVersion.Payload.Data.ToByteArray());
        if (computedCrc32c != parameterVersion.Payload.DataCrc32C)
        {
            throw new InvalidDataException($"Checksum mismatch for {parameterVersion.Name}: data corrupted");
        }

        // Print the verification result and the checksum source.
        Console.WriteLine($"Verified checksum for parameter version: {parameterVersion.Name}");
        Console.WriteLine($"Checksum source: {parameterVersion.ChecksumSource}");

        return parameterVersion;
    }
}
// [END parametermanager_get_param_version_verify_checksum]
