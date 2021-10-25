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

using Google.Cloud.Gaming.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

[CollectionDefinition(nameof(GameServersFixture))]
public class GameServersFixture : IDisposable, IAsyncLifetime, ICollectionFixture<GameServersFixture>
{
    public string ProjectId { get; }
    public string RegionId { get; } = "global";
    public string RealmIdPrefix { get; } = "test-realm";
    public string ClusterIdPrefix { get; } = "test-cluster";
    public string ConfigIdPrefix { get; } = "test-config";
    public string DeploymentIdPrefix { get; } = "test-deployment";
    public string Label1Key { get; } = "label-key-1";
    public string Label1Value { get; } = "label-value-1";
    public string Label2Key { get; } = "label-key-2";
    public string Label2Value { get; } = "label-value-2";
    public string GkeClusterName { get; }

    public List<ClusterIdentifier> ClusterIdentifiers { get; } = new List<ClusterIdentifier>();
    public List<ConfigIdentifier> ConfigIdentifiers { get; } = new List<ConfigIdentifier>();
    public List<string> DeploymentIds { get; } = new List<string>();
    public List<string> RealmIds { get; } = new List<string>();

    public Realm TestRealm { get; set; }
    public string TestRealmId { get; set; }
    public GameServerDeployment TestDeployment { get; set; }
    public string TestDeploymentId { get; set; }
    public GameServerCluster TestCluster { get; set; }
    public string TestClusterId { get; set; }
    public GameServerConfig TestConfig { get; set; }
    public string TestConfigId { get; set; }

    private readonly CreateClusterSample _createClusterSample = new CreateClusterSample();
    private readonly CreateConfigSample _createConfigSample = new CreateConfigSample();
    private readonly CreateRealmSample _createRealmSample = new CreateRealmSample();
    private readonly CreateDeploymentSample _createDeploymentSample = new CreateDeploymentSample();

    private readonly DeleteClusterSample _deleteClusterSample = new DeleteClusterSample();
    private readonly DeleteConfigSample _deleteConfigSample = new DeleteConfigSample();
    private readonly DeleteRealmSample _deleteRealmSample = new DeleteRealmSample();
    private readonly DeleteDeploymentSample _deleteDeploymentSample = new DeleteDeploymentSample();

    public GameServersFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }
        GkeClusterName = Environment.GetEnvironmentVariable("GKE_CLUSTER");
        if (string.IsNullOrEmpty(GkeClusterName))
        {
            GkeClusterName = $"projects/{ProjectId}/locations/us-central1-a/clusters/standard-cluster-1";
        }
    }

    public async Task InitializeAsync()
    {
        TestRealmId = $"{RealmIdPrefix}-{RandomId()}";
        TestRealm = await _createRealmSample.CreateRealmAsync(ProjectId, RegionId, TestRealmId);
        RealmIds.Add(TestRealmId);

        TestDeploymentId = $"{DeploymentIdPrefix}-{RandomId()}";
        TestDeployment = await _createDeploymentSample.CreateDeploymentAsync(ProjectId, TestDeploymentId);
        DeploymentIds.Add(TestDeploymentId);

        TestClusterId = $"{ClusterIdPrefix}-{RandomId()}";
        TestCluster = await _createClusterSample.CreateClusterAsync(ProjectId, RegionId, TestRealmId,
            TestClusterId, GkeClusterName);
        ClusterIdentifiers.Add(new ClusterIdentifier(TestRealmId, TestClusterId));

        TestConfigId = $"{ConfigIdPrefix}-{RandomId()}";
        TestConfig = await _createConfigSample.CreateConfigAsync(ProjectId, RegionId, TestDeploymentId, TestConfigId);
        ConfigIdentifiers.Add(new ConfigIdentifier(TestDeploymentId, TestConfigId));
    }

    public async Task DisposeAsync()
    {
    }

    public void Dispose()
    {
        foreach (ClusterIdentifier id in ClusterIdentifiers)
        {
            try
            {
                _deleteClusterSample.DeleteCluster(ProjectId, RegionId, id.RealmId, id.ClusterId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed for cluster: " + id.ClusterId + " with error: " + e.ToString());
            }
        }
        foreach (ConfigIdentifier id in ConfigIdentifiers)
        {
            try
            {
                _deleteConfigSample.DeleteConfig(ProjectId, RegionId, id.DeploymentId, id.ConfigId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed for config: " + id.ConfigId + " with error: " + e.ToString());
            }
        }
        foreach (string id in RealmIds)
        {
            try
            {
                _deleteRealmSample.DeleteRealm(ProjectId, RegionId, id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed for realm: " + id + " with error: " + e.ToString());
            }
        }
        foreach (string id in DeploymentIds)
        {
            try
            {
                _deleteDeploymentSample.DeleteDeployment(ProjectId, id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed for deployment: " + id + " with error: " + e.ToString());
            }
        }
    }

    public string RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }
}