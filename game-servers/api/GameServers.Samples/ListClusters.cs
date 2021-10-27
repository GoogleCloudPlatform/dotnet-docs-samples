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

// [START cloud_game_servers_cluster_list]

using Google.Api.Gax;
using Google.Cloud.Gaming.V1;
using System.Collections.Generic;
using System.Linq;

public class ListClustersSample
{
    public IList<GameServerCluster> ListClusters(
        string projectId, string regionId, string realmId)
    {
        // Create the client.
        GameServerClustersServiceClient client = GameServerClustersServiceClient.Create();

        ListGameServerClustersRequest request = new ListGameServerClustersRequest
        {
            ParentAsRealmName = RealmName.FromProjectLocationRealm(projectId, regionId, realmId),
            View = GameServerClusterView.Full
        };

        // Make the request.
        PagedEnumerable<ListGameServerClustersResponse, GameServerCluster> response = client.ListGameServerClusters(request);

        // You could iterate through response and write the cluster.Name and

        // cluster.ClusterState to the console to see the installed versions of Agones
        // and Kubernetes on each cluster.

        // The returned sequence will lazily perform RPCs as it's being iterated over.
        return response.ToList();
    }
}
// [END cloud_game_servers_cluster_list]