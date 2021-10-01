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

// [START cloud_game_servers_cluster_create]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Gaming.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

public class CreateClusterSample
{
    public async Task<GameServerCluster> CreateCluster(
        string projectId, string regionId, string realmId, string clusterId, string gkeName)
    {
        // Create the client.
        GameServerClustersServiceClient client = await GameServerClustersServiceClient.CreateAsync();

        GameServerCluster cluster = new GameServerCluster()
        {
            GameServerClusterName = GameServerClusterName.FromProjectLocationRealmCluster(projectId, regionId, realmId, clusterId),
            ConnectionInfo = new GameServerClusterConnectionInfo
            {
                GkeClusterReference = new GkeClusterReference
                {
                    Cluster = gkeName
                },
                Namespace = "default"
            }
        };
        CreateGameServerClusterRequest request = new CreateGameServerClusterRequest
        {
            ParentAsRealmName = RealmName.FromProjectLocationRealm(projectId, regionId, realmId),
            GameServerClusterId = clusterId,
            GameServerCluster = cluster
        };

        // Make the request.
        Operation<GameServerCluster, OperationMetadata> response = await client.CreateGameServerClusterAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<GameServerCluster, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END cloud_game_servers_cluster_create]