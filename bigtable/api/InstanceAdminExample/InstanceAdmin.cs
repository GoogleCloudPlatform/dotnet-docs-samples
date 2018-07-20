// Copyright 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using CommandLine;
using Google.LongRunning;
using Google.Cloud.Bigtable.Admin.V2;
using System.Text.RegularExpressions;

namespace GoogleCloudSamples.Bigtable
{
    [Verb("createProdInstance", HelpText = "Create a `PRODUCTION` type Instance with SSD storage type in this project.")]
    class CreateProdInstanceOptions
    {
        [Value(1, HelpText = "The displayName for an instance to create.", Required = true)]
        public string displayName { get; set; }
    }

    [Verb("createDevInstance", HelpText = "Create a `DEVELOPMENT` type Instance with HDD storage type in this project.")]
    class CreateDevInstanceOptions
    {
        [Value(1, HelpText = "The displayName for an instance to create.", Required = true)]
        public string displayName { get; set; }
    }

    [Verb("listInstances", HelpText = "Lists instances in a project.")]
    class ListInstancesOptions
    {
    }

    [Verb("getInstance", HelpText = "Gets information about an instance in a project.")]
    class GetInstanceOptions
    {
        [Value(1, HelpText = "The instanceId to get.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("listClusters", HelpText = "Lists clusters in an instance.")]
    class ListClustersOptions
    {
        [Value(1, HelpText = "The instanceId of the instance for which a list of clusters is requested.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("createCluster", HelpText = "Creates an additional replicated cluster within an instance.")]
    class CreateClusterOptions
    {
        [Value(1, HelpText = "The instanceId to which cluster belongs.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("deleteCluster", HelpText = "Deletes a cluster from an instance.")]
    class DeleteClusterOptions
    {
        [Value(1, HelpText = "The instanceId to which cluster belongs.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("deleteInstance", HelpText = "Deletes an instance from a project.")]
    class DeleteInstanceOptions
    {
        [Value(1, HelpText = "The instanceId of the instance to delete.", Required = true)]
        public string instanceId { get; set; }
    }

    public class InstanceAdmin
    {
        // Your Google Cloud Platform project ID
        private const string projectId = "YOUR-PROJECT-ID";

        public static object CreateProdInstance(string displayName)
        {
            // [START bigtable_create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END bigtable_create_bigtableInstanceAdminClient]

            Console.WriteLine("Creating a PRODUCTION instance");
            // [START bigtable_create_prod_instance]
            // Creates a Production Instance with "<intanceId>-prod" instance id
            // with cluster id "ssd-cluster1", 3 nodes and location us-east1-b.
            displayName += " Prod"; // display name is for display purposes only, it doesn't have to equal to instanceId and can be amended after instance is created.
            string instanceId = Regex.Replace(displayName, @"[^A-Za-z0-9_\.~]+", "-").ToLower();

            // Please refer to the link below for the full list of availabel locations:
            // https://cloud.google.com/bigtable/docs/locations
            string zone1 = "us-east1-b";

            // The instance to create.
            Instance myInstance = new Instance
            {
                DisplayName = displayName,
                // You can choose DEVELOPMENT or PRODUCTION type here.
                // If not set, will default to PRODUCTION type.
                // Instance type can be upgraded from DEVELOPMENT to PRODUCTION but can not be dowgraded after instance is created.
                Type = Instance.Types.Type.Production,
                Labels = { { "prod-label", "prod-label" } }
            };

            // The first cluster to be created within the instance.
            Cluster myCluster1 = new Cluster
            {
                // You can choose SSD or HDD storage type here: StorageType.Ssd or StorageType.Hdd.
                // Cluster storage type can not be changed after instance is created.
                // If not set will default to SSD type.
                DefaultStorageType = StorageType.Ssd,
                LocationAsLocationName = new LocationName(projectId, zone1),
                // Serve Nodes count can only be set if PRODUCTION type instance is being created (refer to line 297 above) 
                // Minimum count of 3 serve nodes must be specified.
                // Serve Nodes count can be increased and decreased after instance is created.
                ServeNodes = 3
            };

            // Initialize request argument(s).
            CreateInstanceRequest request = new CreateInstanceRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
                Instance = myInstance,
                InstanceId = instanceId,
                // Must specify at lease one cluster.
                // Only PRODUCTION type instance can be created with more than one cluster.
                // Currently all clusters must have the same storage type.
                // Clusters must be set to different locations.
                Clusters = { { "ssd-cluster1", myCluster1 } }
            };

            try
            {
                // Make a request.
                Operation<Instance, CreateInstanceMetadata> createInstanceResponse =
                    bigtableInstanceAdminClient.CreateInstance(request);
                Console.WriteLine("Waiting for operation to complete...");

                // Poll until the returned long-running operation is complete
                Operation<Instance, CreateInstanceMetadata> completedResponse =
                    createInstanceResponse.PollUntilCompleted();
                // [END bigtable_create_prod_instance]
                Console.WriteLine(
                    $"Instance: {displayName} {(completedResponse.IsCompleted ? "was successfully created in " : "failed to create in ")}{projectId} project");
                PrintInstanceInfo(completedResponse.Result);
                // [START bigtable_create_prod_instance]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating {displayName} instance");
                Console.WriteLine(ex.Message);
            }
            // [END bigtable_create_prod_instance]
            return 0;
        }

        public static object CreateDevInstance(string displayName)
        {
            // [START bigtable_create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END bigtable_create_bigtableInstanceAdminClient]

            Console.WriteLine("Creating a DEVELOPMENT instance");
            // [START bigtable_create_dev_instance]
            // Creates a DEVELOPMENT Instance with "<intanceId>-dev" instance id
            // with cluster id "hdd-cluster" and location us-east1-b.
            // Cluster node count should not be set while creating DEVELOPMENT instance.
            displayName += " Dev"; // display name is for display purposes only, it doesn't have to equal to instanceId and can be amended after instance is created.
            string instanceId = Regex.Replace(displayName, @"[^A-Za-z0-9_\.~]+", "-").ToLower();

            // Please refer to the link below for the full list of availabel locations:
            // https://cloud.google.com/bigtable/docs/locations
            string zone = "us-east1-b";

            // The instance to create.
            Instance myInstance = new Instance
            {
                DisplayName = displayName,
                // You can choose DEVELOPMENT or PRODUCTION type here.
                // If not set, will default to PRODUCTION type.
                // Instance type can be upgraded from DEVELOPMENT to PRODUCTION but can not be dowgraded after instance is created.
                Type = Instance.Types.Type.Development,
                Labels = { { "dev-label", "dev-label" } }
            };

            // The first cluster to be created within the instance.
            Cluster myCluster = new Cluster
            {
                // You can choose SSD or HDD storage type here: StorageType.Ssd or StorageType.Hdd.
                // Cluster storage type can not be changed after instance is created.
                // If not set will default to SSD type.
                DefaultStorageType = StorageType.Hdd,
                LocationAsLocationName = new LocationName(projectId, zone),
            };

            // Initialize request argument(s).
            CreateInstanceRequest request = new CreateInstanceRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
                Instance = myInstance,
                InstanceId = instanceId,
                // Must specify at lease one cluster.
                // Only PRODUCTION type instance can be created with more than one cluster.
                Clusters = { { "hdd-cluster", myCluster } }
            };

            try
            {
                // Make a request.
                Operation<Instance, CreateInstanceMetadata> createInstanceResponse =
                    bigtableInstanceAdminClient.CreateInstance(request);
                Console.WriteLine("Waiting for operation to complete...");

                // Poll until the returned long-running operation is complete
                Operation<Instance, CreateInstanceMetadata> completedResponse =
                    createInstanceResponse.PollUntilCompleted();
                // [END bigtable_create_dev_instance]
                Console.WriteLine(
                    $"Instance: {displayName} {(completedResponse.IsCompleted ? "was successfully created in " : "failed to create in ")}{projectId} project");
                PrintInstanceInfo(completedResponse.Result);
                // [START bigtable_create_dev_instance]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating {displayName} instance");
                Console.WriteLine(ex.Message);
            }
            // [END bigtable_create_dev_instance]
            return 0;
        }

        public static object ListInstances()
        {
            // [START bigtable_create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END bigtable_create_bigtableInstanceAdminClient]

            Console.WriteLine($"Listing Instances in the project {projectId}");
            // [START bigtable_list_instances]
            // Lists instances in the project.
            // Initialize request argument(s).
            ListInstancesRequest listInstancesRequest = new ListInstancesRequest
            {
                ParentAsProjectName = new ProjectName(projectId)
            };
            try
            {
                // Make a request.
                Console.WriteLine("Waiting for operation to complete...");
                ListInstancesResponse instances = bigtableInstanceAdminClient.ListInstances(listInstancesRequest);
                // [END bigtable_list_instances]
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"{"Instance Count:",-30}{instances.Instances.Count} instances in project {projectId}");
                foreach (Instance inst in instances.Instances)
                {
                    PrintInstanceInfo(inst);
                }
                // [START bigtable_list_instances]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while requesting information about instances in {projectId} project");
                Console.WriteLine(ex.Message);
                return -1;
            }
            Console.WriteLine(new string('-', 50));
            // [END bigtable_list_instances]
            return 0;
        }

        public static object GetInstance(string instanceId)
        {
            // [START bigtable_create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END bigtable_create_bigtableInstanceAdminClient]

            Console.WriteLine("Getting information about an instance");
            // [START bigtable_get_instance]
            // Initialize request argument(s).
            GetInstanceRequest request = new GetInstanceRequest
            {
                InstanceName = new InstanceName(projectId, instanceId)
            };
            try
            {
                // Make Request.
                Console.WriteLine("Waiting for operation to complete...");
                Instance respond = bigtableInstanceAdminClient.GetInstance(request);
                // [END bigtable_get_instance]
                PrintInstanceInfo(respond);
                // [START bigtable_get_instance]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception retreiving {instanceId} instance");
                Console.WriteLine(ex.Message);
            }
            // [END bigtable_get_instance]
            return 0;
        }

        public static object ListClusters(string instanceId)
        {
            // [START bigtable_create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END bigtable_create_bigtableInstanceAdminClient]

            Console.WriteLine($"Listing clusters on instance {instanceId}");
            // [START bigtable_list_clusters]
            // Lists clusters in the instance.
            // Initialize request argument(s)
            ListClustersRequest listClustersRequest = new ListClustersRequest
            {
                ParentAsInstanceName = new InstanceName(projectId, instanceId)
            };

            try
            {
                // Make a request.
                Console.WriteLine("Waiting for operation to complete...");
                ListClustersResponse response = bigtableInstanceAdminClient.ListClusters(listClustersRequest);
                // [END bigtable_list_clusters]
                Console.WriteLine($"{"Cluster count:",-30}{response.Clusters.Count} clusters on instance {instanceId}\n");
                foreach (Cluster clstr in response.Clusters)
                {
                    PrintClusterInfo(clstr);
                }
                // [START bigtable_list_clusters]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while requesting information about clusters in {instanceId} instance");
                Console.WriteLine(ex.Message);
            }
            // [END bigtable_list_clusters]

            return 0;
        }

        public static object CreateCluster(string instanceId)
        {
            // [START bigtable_create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END bigtable_create_bigtableInstanceAdminClient]

            Console.WriteLine("Print current instance information");
            GetInstance(instanceId);

            // Please refer to the link below for the full list of availabel locations:
            // https://cloud.google.com/bigtable/docs/locations
            string zone2 = "us-east1-d";

            Console.WriteLine("Creating cluster");
            // [START bigtable_create_cluster]
            // Create an additional cluster with cluster id "ssd-cluster2" with 3 nodes and location us-east1-d.
            // Additional cluster can only be created in PRODUCTION type instance.
            // Additional cluster must have same storage type as existing cluster.
            // Please read about routing_policy for more information on mutli cluster instances.
            // https://cloud.google.com/bigtable/docs/reference/admin/rpc/google.bigtable.admin.v2#google.bigtable.admin.v2.AppProfile.MultiClusterRoutingUseAny
            // Cluster to be created within the instance.
            Cluster myCluster2 = new Cluster
            {
                DefaultStorageType = StorageType.Ssd,
                LocationAsLocationName = new LocationName(projectId, zone2),
                ServeNodes = 3
            };
            // Initialize request argument(s).
            CreateClusterRequest request = new CreateClusterRequest
            {
                ParentAsInstanceName = new InstanceName(projectId, instanceId),
                ClusterId = "ssd-cluster2",
                Cluster = myCluster2
            };
            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                Operation<Cluster, CreateClusterMetadata> response = bigtableInstanceAdminClient.CreateCluster(request);
                // Poll until the returned long-running operation is complete
                Operation<Cluster, CreateClusterMetadata> completedResponse = response.PollUntilCompleted();

                // [END bigtable_create_cluster]
                Console.WriteLine($"Cluster {request.ClusterId} was created successfully in instance {instanceId}");
                Console.WriteLine("Print intance information after cluster is created");
                GetInstance(instanceId);
                // [START bigtable_create_cluster]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception creating additional cluster {request.ClusterId} in instance {instanceId}");
                Console.WriteLine(ex.Message);
            }
            // [END bigtable_create_cluster]

            return 0;
        }

        public static object DeleteCluster(string instanceId)
        {
            // [START bigtable_create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END bigtable_create_bigtableInstanceAdminClient]

            Console.WriteLine("Print current instance information");
            GetInstance(instanceId);

            Console.WriteLine("Deleting cluster");
            // [START bigtable_delete_cluster]
            // Deltes cluster "ssd-cluster2" from instance.
            // At least one cluster must remain on an instance.
            // Initialize request argument(s)
            DeleteClusterRequest request = new DeleteClusterRequest
            {
                ClusterName = new ClusterName(projectId, instanceId, "ssd-cluster2")
            };
            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                bigtableInstanceAdminClient.DeleteCluster(request);
                // [END bigtable_delete_cluster]
                Console.WriteLine($"Cluster {request.ClusterName.ClusterId} was deleted successfully from instance {instanceId}");
                Console.WriteLine("Print intance information after cluster is deleted");
                GetInstance(instanceId);
                // [START bigtable_delete_cluster]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception deleting cluster {request.ClusterName.ClusterId} from instance {instanceId}");
                Console.WriteLine(ex.Message);
            }
            // [END bigtable_delete_cluster]
            return 0;
        }

        public static object DeleteInstance(string instanceId)
        {
            // [START bigtable_create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END bigtable_create_bigtableInstanceAdminClient]

            Console.WriteLine("Print list of instances in the project");
            ListInstances();

            Console.WriteLine("Deleting Instance");
            // [START bigtable_delete_instance]
            // Deletes an instance from the project.
            // Initialize request argument(s).
            DeleteInstanceRequest request = new DeleteInstanceRequest
            {
                InstanceName = new InstanceName(projectId, instanceId)
            };
            try
            {
                // Make request.
                Console.WriteLine("Waiting for operation to complete...");
                bigtableInstanceAdminClient.DeleteInstance(request);
                // [END bigtable_delete_instance]
                Console.WriteLine($"Instance {instanceId} deleted successfuly");
                Console.WriteLine("Print list of instances in the project after instance is deleted");
                ListInstances();
                // [START bigtable_delete_instance]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while deleting {instanceId} instance");
                Console.WriteLine(ex.Message);
            }
            // [END bigtable_delete_instance]
            return 0;
        }

        private static void PrintInstanceInfo(Instance instance)
        {
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Printing instance {instance.InstanceName.InstanceId}");
            Console.WriteLine(
                $"{"Instance ID:",-30}{instance.InstanceName.InstanceId}\n{"Instance Display Name:",-30}{instance.DisplayName}\n{"Type:",-30}{instance.Type}\n{"State:",-30}{instance.State}");
            if (instance.Labels.Any())
            {
                Console.WriteLine($"Printing instance {instance.InstanceName.InstanceId}");
                Console.WriteLine($"{"Label Count:",-30}{instance.Labels.Count}");
                foreach (string labelsKey in instance.Labels.Keys)
                {
                    Console.WriteLine($"{"{",26}{labelsKey} : {instance.Labels[labelsKey]}}}");
                }
            }
            ListClusters(instance.InstanceName.InstanceId);
        }

        private static void PrintClusterInfo(Cluster cluster)
        {
            Console.WriteLine($"Printing cluster {cluster.ClusterName.ClusterId}");
            Console.WriteLine(
                $"{"  Cluster ID:",-30}{cluster.ClusterName.ClusterId}\n{"  Storage Type:",-30}{cluster.DefaultStorageType}" +
                $"\n{"  Location:",-30}{cluster.LocationAsLocationName.LocationId}\n{"  Node Count:",-30}{cluster.ServeNodes}\n{"  State:",-30}{cluster.State}\n");
        }

        public static int Main(string[] args)
        {
            if (projectId == "YOUR-PROJECT" + "-ID")
            {
                Console.WriteLine("Edit InstanceAdmin.cs and replace YOUR-PROJECT-ID with your project id.");
                return -1;
            }

            Parser.Default.ParseArguments<
                    CreateProdInstanceOptions, CreateDevInstanceOptions,
                    ListInstancesOptions, GetInstanceOptions,
                    ListClustersOptions, CreateClusterOptions,
                    DeleteClusterOptions, DeleteInstanceOptions>(args)
                .MapResult(
                    (CreateProdInstanceOptions opts) => CreateProdInstance(opts.displayName),
                    (CreateDevInstanceOptions opts) => CreateDevInstance(opts.displayName),
                    (ListInstancesOptions opts) => ListInstances(),
                    (GetInstanceOptions opts) => GetInstance(opts.instanceId),
                    (ListClustersOptions opts) => ListClusters(opts.instanceId),
                    (CreateClusterOptions opts) => CreateCluster(opts.instanceId),
                    (DeleteClusterOptions opts) => DeleteCluster(opts.instanceId),
                    (DeleteInstanceOptions opts) => DeleteInstance(opts.instanceId), errs => 1);
            return 0;
        }
    }
}
