// Copyright(c) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using CommandLine;
using Google.Cloud.Gaming.V1Beta;
using System;

namespace GoogleCloudSamples
{
    public class GameServerClusterOptions : RealmOptions
    {
        [Value(3, HelpText = "ID for the cluster")]
        public string ClusterId { get; set; }
    }

    [Verb("create_cluster", HelpText = "Create a cluster")]
    public class CreateGameServerClusterOptions : GameServerClusterOptions
    {
        [Value(4, HelpText = "GKE cluster ID for the game server cluster")]
        public string GkeClusterName { get; set; }
    }

    [Verb("get_cluster", HelpText = "Get a cluster")]
    public class GetGameServerClusterOptions : GameServerClusterOptions
    { }

    [Verb("list_clusters", HelpText = "List all clusters in a realm")]
    public class ListGameServerClustersOptions : RealmOptions
    { }

    [Verb("delete_cluster", HelpText = "Delete a cluster")]
    public class DeleteGameServerClusterOptions : GameServerClusterOptions
    { }

    public class GameServerClusters
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((CreateGameServerClusterOptions opts) => CreateGameServerCluster(opts.ProjectID,
                opts.Location, opts.RealmId, opts.ClusterId, opts.GkeClusterName))
                .Add((ListGameServerClustersOptions opts) => ListGameServerClusters(opts.ProjectID,
                    opts.Location, opts.RealmId))
                .Add((GetGameServerClusterOptions opts) => GetGameServerCluster(opts.ProjectID,
                    opts.Location, opts.RealmId, opts.ClusterId))
                .Add((DeleteGameServerClusterOptions opts) => DeleteGameServerCluster(opts.ProjectID,
                    opts.Location, opts.RealmId, opts.ClusterId));
        }

        public static object CreateGameServerCluster(string projectID,
            string location, string realmId, string clusterId,
            string gkeClusterName)
        {
            var client = GameServerClustersServiceClient.Create();
            var request = new CreateGameServerClusterRequest
            {
                ParentAsRealmName = new RealmName(projectID, location, realmId),
                GameServerClusterId = clusterId,
                GameServerCluster = new GameServerCluster
                {
                    Description = "My Game Server Cluster",
                    ConnectionInfo = new GameServerClusterConnectionInfo
                    {
                        GkeClusterReference = new GkeClusterReference
                        {
                            Cluster = gkeClusterName
                        },
                        Namespace = "default",
                    },
                },
            };

            var operation = client.CreateGameServerCluster(request);

            // Synchronous check of operation status
            var completedResponse = operation.PollUntilCompleted();

            if (completedResponse.IsCompleted)
            {
                var result = completedResponse.Result;

                Console.WriteLine($"Cluster name: {result.Name}");
                Console.WriteLine($"Cluster description: {result.Description}");
                Console.WriteLine($"GKE cluster:" +
                    $"{result.ConnectionInfo.GkeClusterReference.Cluster}");
            }
            return 0;
        }

        public static object ListGameServerClusters(string projectID,
            string location, string realmId)
        {
            var client = GameServerClustersServiceClient.Create();
            var request = new ListGameServerClustersRequest
            {
                ParentAsRealmName = new RealmName(projectID, location, realmId)
            };

            var response = client.ListGameServerClusters(request);

            foreach (var cluster in response)
            {
                Console.WriteLine($"Cluster name: {cluster.Name}");
                Console.WriteLine($"Cluster description: {cluster.Description}");
                Console.WriteLine($"GKE cluster:" +
                    $"{cluster.ConnectionInfo.GkeClusterReference.Cluster}" +
                    Environment.NewLine);
            }

            return 0;
        }

        public static object GetGameServerCluster(string projectID,
            string location, string realmId, string clusterId)
        {
            var client = GameServerClustersServiceClient.Create();
            var request = new GetGameServerClusterRequest
            {
                GameServerClusterName = new GameServerClusterName(projectID,
                                            location, realmId, clusterId)
            };

            var cluster = client.GetGameServerCluster(request);
            Console.WriteLine($"Cluster name: {cluster.Name}");
            Console.WriteLine($"Cluster description: {cluster.Description}");
            Console.WriteLine($"GKE cluster:" +
                $"{cluster.ConnectionInfo.GkeClusterReference.Cluster}");
            return 0;
        }

        public static object DeleteGameServerCluster(string projectID,
            string location, string realmId, string clusterId)
        {
            var client = GameServerClustersServiceClient.Create();
            var request = new DeleteGameServerClusterRequest
            {
                GameServerClusterName = new GameServerClusterName(projectID,
                                        location, realmId, clusterId),
            };

            var operation = client.DeleteGameServerCluster(request);

            // Synchronous check of operation status
            var completedResponse = operation.PollUntilCompleted();
            if (completedResponse.IsCompleted)
            {
                Console.WriteLine("Cluster deleted.");
            }

            return 0;
        }
    }
}
