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

// [START cloud_game_servers_cluster_get]

using Google.Cloud.Gaming.V1;

public class GetClusterSample
{
    public GameServerCluster GetCluster(
        string projectId, string regionId, string realmId, string clusterId)
    {
        // Create the client.
        GameServerClustersServiceClient client = GameServerClustersServiceClient.Create();

        GetGameServerClusterRequest request = new GetGameServerClusterRequest
        {
            GameServerClusterName = GameServerClusterName.FromProjectLocationRealmCluster(projectId, regionId, realmId, clusterId),
            View = GameServerClusterView.Full
        };

        // Make the request.
        GameServerCluster response = client.GetGameServerCluster(request);

        // You could write response.Name and response.ClusterState to the console
        // to see the installed versions of Agones and Kubernetes on the cluster.

        return response;
    }
}
// [END cloud_game_servers_cluster_get]
