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

using Google;
using GoogleCloudSamples;
using Google.Cloud.Gaming.V1;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;
using Xunit;
using Xunit.Sdk;

[CollectionDefinition(nameof(GameServersFixture))]
public class GameServersFixture : IDisposable, ICollectionFixture<GameServersFixture>
{
    public string ProjectId { get; }
    public string RegionId { get; } = "global";
    public string RealmId { get; } = "test-realm";
    public string Label1Key { get; } = "label-key-1";
    public string Label1Value { get; } = "label-value-1";
    public string Label2Key { get; } = "label-key-2";
    public string Label2Value { get; } = "label-value-2";
    public string GkeClusterName { get; }

    public List<ClusterIdentifierUtil> ClusterIdentifiers { get; } = new List<ClusterIdentifierUtil>();
    public List<ConfigIdentifierUtil> ConfigIdentifiers { get; } = new List<ConfigIdentifierUtil>();
    public List<string> DeploymentIds { get; } = new List<string>();
    public List<string> RealmIds { get; } = new List<string>();

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

    public void Dispose()
    {
        foreach (ClusterIdentifierUtil id in ClusterIdentifiers)
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
        foreach (ConfigIdentifierUtil id in ConfigIdentifiers)
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