/*
 * Copyright 2023 Google LLC
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

// [START livestream_update_pool]

using Google.Cloud.Video.LiveStream.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

public class UpdatePoolSample
{
    public async Task UpdatePoolAsync(
         string projectId, string locationId, string poolId, string peeredNetwork)
    {
        // Create the client.
        LivestreamServiceClient client = LivestreamServiceClient.Create();

        UpdatePoolRequest request = new UpdatePoolRequest
        {
            Pool = new Pool
            {
                PoolName = PoolName.FromProjectLocationPool(projectId, locationId, poolId),
                NetworkConfig = new Pool.Types.NetworkConfig
                {
                    PeeredNetwork = peeredNetwork
                }
            },
            UpdateMask = new FieldMask { Paths = { "network_config" } }
        };

        // Make the request.
        Operation<Pool, OperationMetadata> response = await client.UpdatePoolAsync(request);
        // Get the name of the operation.
        string operationName = response.Name;
        // This name can be stored, then the long-running operation retrieved later by name.
        Operation<Pool, OperationMetadata> retrievedResponse = await client.PollOnceUpdatePoolAsync(operationName);
        // Check if the retrieved long-running operation has completed.
        if (retrievedResponse.IsCompleted)
        {
            // If it has completed, then access the result.
            _ = retrievedResponse.Result;
        }
    }
}
// [END livestream_update_pool]