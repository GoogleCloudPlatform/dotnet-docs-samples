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

using Google.Cloud.Gaming.V1Beta;
using Google.Api.Gax.ResourceNames;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace GoogleCloudSamples
{
    public class GameServersTestsBase
    {
        protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        protected string GKEClusterId { get; private set; } = Environment.GetEnvironmentVariable("SAMPLE_CLUSTER_NAME");
        protected string RegionName { get; private set; } = "us-central1";
        protected string RealmId { get; private set; } = "fake-realm-for-test";
        protected string GameServerClusterId { get; private set; } = "fake-cluster-for-test";

        private readonly CommandLineRunner _gameServers = new CommandLineRunner()
        {
            Main = GameServersProgram.Main,
            Command = "Game Servers"
        };

        public GameServersTestsBase()
        {
            var exceptions = new List<Exception>();
            if (string.IsNullOrEmpty(this.ProjectId))
            {
                exceptions.Add(new Exception("GOOGLE_PROJECT_ID environment variable not set."));
            }
            if (string.IsNullOrEmpty(this.GKEClusterId))
            {
                exceptions.Add(new Exception("SAMPLE_CLUSTER_NAME environment variable not set."));
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        public ConsoleOutput Run(params string[] arguments)
        {
            return _gameServers.Run(arguments);
        }

        /// <summary>
        /// Delete all realms and clusters created by tests.
        /// </summary>
        protected void DeleteAllResources()
        {
            var realmClient = RealmsServiceClient.Create();
            var clusterClient = GameServerClustersServiceClient.Create();
            var exceptions = new List<Exception>();

            var listOfClusters = clusterClient.ListGameServerClusters(new ListGameServerClustersRequest
            {
                ParentAsRealmName = new RealmName(ProjectId, RegionName, RealmId)
            });

            try
            {
                foreach (var cluster in listOfClusters)
                {
                    Run("delete_cluster", ProjectId, RegionName, RealmId, cluster.GameServerClusterName.ClusterId);
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }

            var listOfRealms = realmClient.ListRealms(new ListRealmsRequest
            {
                ParentAsLocationName = new LocationName(ProjectId, RegionName)
            });

            try
            {
                foreach (var realm in listOfRealms)
                {
                    if (realm.RealmName.RealmId.Contains(RealmId))
                    {
                        Run("delete_realm", ProjectId, RegionName, RealmId);
                    }
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Add a random chunk to all the Ids used in the tests, so that
        /// multiple machines can run the same tests at the same time
        /// in the same Google Cloud project without interfering with each
        /// other.
        /// </summary>
        protected void RandomizeIds()
        {
            RealmId += TestUtil.RandomName();
            GameServerClusterId += TestUtil.RandomName();
        }
    }

    public class GameServersTests : GameServersTestsBase, IDisposable
    {
        public GameServersTests()
        {
            RandomizeIds();
        }

        [Fact]
        public void TestCreateRealm()
        {
            var output = Run("create_realm", ProjectId, RegionName, RealmId);
            Assert.Contains("Realm name", output.Stdout);
        }

        [Fact]
        public void TestListRealms()
        {
            Run("create_realm", ProjectId, RegionName, RealmId);

            var output = Run("list_realms", ProjectId, RegionName);
            Assert.Contains("Realm name", output.Stdout);
        }

        [Fact]
        public void TestGetRealm()
        {
            Run("create_realm", ProjectId, RegionName, RealmId);

            var output = Run("get_realm", ProjectId, RegionName, RealmId);
            Assert.Contains("Realm name", output.Stdout);
        }

        [Fact]
        public void TestDeleteRealm()
        {
            Run("create_realm", ProjectId, RegionName, RealmId);

            var output = Run("delete_realm", ProjectId, RegionName, RealmId);
            Assert.Contains("Realm deleted", output.Stdout);
        }

        [Fact]
        public void TestCreateGameServerCluster()
        {
            Run("create_realm", ProjectId, RegionName, RealmId);

            var output = Run("create_cluster", ProjectId, RegionName, RealmId, GameServerClusterId, GKEClusterId);
            Assert.Contains("Cluster name", output.Stdout);
        }

        [Fact]
        public void TestListGameServerClusters()
        {
            Run("create_realm", ProjectId, RegionName, RealmId);
            Run("create_cluster", ProjectId, RegionName, RealmId, GameServerClusterId, GKEClusterId);

            var output = Run("list_clusters", ProjectId, RegionName, RealmId);
            Assert.Contains("Cluster name", output.Stdout);
        }

        [Fact]
        public void TestGetGameServerCluster()
        {
            Run("create_realm", ProjectId, RegionName, RealmId);
            Run("create_cluster", ProjectId, RegionName, RealmId, GameServerClusterId, GKEClusterId);

            var output = Run("get_cluster", ProjectId, RegionName, RealmId, GameServerClusterId);
            Assert.Contains("Cluster name", output.Stdout);
        }

        [Fact]
        public void TestDeleteGameServerCluster()
        {
            Run("create_realm", ProjectId, RegionName, RealmId);
            Run("create_cluster", ProjectId, RegionName, RealmId, GameServerClusterId, GKEClusterId);

            var output = Run("delete_cluster", ProjectId, RegionName, RealmId, GameServerClusterId);
            Assert.Contains("Cluster deleted", output.Stdout);
        }

        public void Dispose()
        {
            DeleteAllResources();
        }
    }
}
