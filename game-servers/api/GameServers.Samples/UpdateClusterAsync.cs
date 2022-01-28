/*
 * Copyright 2021 Google LLC
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

// [START cloud_game_servers_cluster_update]

using Google.Cloud.Gaming.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

public class UpdateClusterSample
{
    public async Task<GameServerCluster> UpdateClusterAsync(
        string projectId, string regionId, string realmId, string clusterId)
    {
        // Create the client.
        GameServerClustersServiceClient client = await GameServerClustersServiceClient.CreateAsync();

        GameServerCluster cluster = new GameServerCluster()
        {
            GameServerClusterName = GameServerClusterName.FromProjectLocationRealmCluster(projectId, regionId, realmId, clusterId)
        };
        cluster.Labels.Add("label-key-1", "label-value-1");
        cluster.Labels.Add("label-key-2", "label-value-2");

        UpdateGameServerClusterRequest request = new UpdateGameServerClusterRequest
        {
            GameServerCluster = cluster,
            UpdateMask = new FieldMask { Paths = { "labels" } }
        };

        // Make the request.
        Operation<GameServerCluster, OperationMetadata> response = await client.UpdateGameServerClusterAsync(request);
        Operation<GameServerCluster, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result. This result will NOT contain the updated labels.
        // If you want to get the updated resource, use a GET request on the resource.
        return completedResponse.Result;
    }
}
// [END cloud_game_servers_cluster_update]