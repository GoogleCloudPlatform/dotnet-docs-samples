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

// [START parametermanager_create_param_version_with_checksum]

using Force.Crc32;
using Google.Cloud.ParameterManager.V1;
using Google.Protobuf;
using System.Text;

public class CreateParameterVersionWithChecksumSample
{
    /// <summary>
    /// Creates a parameter version with a CRC32C checksum of the payload.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="parameterId">The ID of the parameter.</param>
    /// <param name="versionId">The ID of the parameter version.</param>
    public ParameterVersion CreateParameterVersionWithChecksum(
        string projectId,
        string parameterId,
        string versionId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the parent resource name.
        ParameterName parent = new ParameterName(projectId, "global", parameterId);

        // Build the payload and compute its checksum.
        byte[] payload = Encoding.UTF8.GetBytes("hello world!");
        uint dataCrc32c = Crc32CAlgorithm.Compute(payload);

        // Build the parameter version with the checksum.
        ParameterVersion parameterVersion = new ParameterVersion
        {
            Payload = new ParameterVersionPayload
            {
                Data = ByteString.CopyFrom(payload),
                DataCrc32C = dataCrc32c
            }
        };

        // Call the API.
        ParameterVersion createdParameterVersion = client.CreateParameterVersion(parent, parameterVersion, versionId);

        // Print the created parameter version name and checksum source.
        Console.WriteLine($"Created parameter version: {createdParameterVersion.Name}");
        Console.WriteLine($"Checksum source: {createdParameterVersion.ChecksumSource}");

        return createdParameterVersion;
    }
}
// [END parametermanager_create_param_version_with_checksum]
