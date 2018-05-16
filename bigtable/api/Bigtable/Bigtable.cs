using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;
using Google.LongRunning;
using Google.Protobuf.Collections;
using Google.Cloud.Bigtable.V2;
using Google.Cloud.Bigtable.Admin.V2;
using Google.Protobuf.WellKnownTypes;

namespace GoogleCloudSamples.Bigtable
{
    [Verb("createProdSsdInstance", HelpText = "Create a `PRODUCTION` type Instance with SSD storage type in this project.")]
    class CreateProdSsdInstanceOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId to create.", Required = true)]
        public string displayName { get; set; }
    }

    [Verb("createDevHddInstance", HelpText = "Create a `DEVELOPMENT` type Instance with HDD storage type in this project.")]
    class CreateDevHddInstanceOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId to create.", Required = true)]
        public string displayName { get; set; }
    }

    [Verb("createTwoClusterInstance", HelpText =
        "Create an instance with two replicated clusters in different parts of the same zone. " +
        "Only `PRODUCTION` type instance supports multiple replicated clusters.")]
    class CreateTwoClusterInstanceOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId to create.", Required = true)]
        public string displayName { get; set; }
    }

    [Verb("listInstances", HelpText = "Lists information about instances in a project.")]
    class ListInstancesOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
    }

    [Verb("getInstance", HelpText = "Gets information about an instance in a project.")]
    class GetInstanceOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId to get.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("partialUpdateInstance", HelpText = "Partially updates an instance within a project.")]
    class PartialUpdateInstanceOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to be updated.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The new displayName for the instance.", Required = false)]
        public string displayName { get; set; }
        [Value(3, HelpText = "If current instance is a `DEVELOPMENT` type, can be upgraded to `PRODUCTION", Required = false)]
        public bool uppgradeToProd { get; set; }
        //public Instance.Types.Type instanceType { get; set; }
        [Value(4, HelpText = "Labels as a mechanism for organizing cloud resources into groups.", Required = false)]
        public MapField<string, string> labels { get; set; }
    }

    [Verb("updateInstanceDisplayName", HelpText = "Change a display name of an exisitng instance")]
    class UpdateInstanceDisplayNameOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to be updated.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The new displayName for the instance.", Required = false)]
        public string newDisplayName { get; set; }
    }

    [Verb("upgradeInstanceToProd", HelpText = "Change a display name of an exisitng instance")]
    class UpgradeInstanceToProdOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to be upgraded.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("addLabels", HelpText = "Add a label to an existing instance")]
    class AddLabelsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to be updated.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("updateLabels", HelpText = "Update exisitng labels")]
    class UpdateLabelsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to be updated.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("updateInstanceMultipleFields", HelpText = "Change a display name and update labels of an existing instance in one request")]
    class UpdateInstanceMultipleFieldsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to be updated.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The new displayName for the instance.", Required = false)]
        public string newDisplayName { get; set; }
    }

    [Verb("deleteInstance", HelpText = "Delete an instance from a project.")]
    class DeleteInstanceOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to delete.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("createCluster", HelpText = "Creates an additional replicated cluster within an instance.")]
    class CreateClusterOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId to which cluster belongs.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("getCluster", HelpText = "Gets information about a cluster.")]
    class GetClusterOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId to which cluster belongs.", Required = true)]
        public string instanceId { get; set; }
        [Value(3, HelpText = "ClusterId to being requested.", Required = true)]
        public string clusterId { get; set; }
    }

    [Verb("listAllClusters", HelpText = "List information about all clusters in a project")]
    class ListAllClustersOptions 
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
    }

    [Verb("listClusters", HelpText = "Lists information about clusters in an instance.")]
    class ListClustersOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance for which a list of clusters is requested.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("updateClusterNodeCount", HelpText = "Updates Node count in a cluster within an instance.")]
    class UpdateClusterNodeCountOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId to which cluster belongs.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "ClusterId to be updated.", Required = true)]
        public string clusterId { get; set; }
        [Value(3, HelpText = "New number of nodes allocated to this cluster. Only applicable to a `PRODUCTION` type instance.", Required = true)]
        public int nodes { get; set; }
    }

    class Bigtable
    {
        public static object CreateProdSsdInstance(string projectId, string displayName)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START Create_Ssd_Instance]
            displayName += " SSD Prod";
            string instanceId = Regex.Replace(displayName, @"[^A-Za-z0-9_\.~]+", "-").ToLower();
            string zone = "us-east1-b";
            // The instance to create.
            Instance myInstance = new Instance
            {
                DisplayName = displayName,
                Type = Instance.Types.Type.Production
            };
            //The cluster to be created within the instance.
            Cluster myCluster = new Cluster
            {
                DefaultStorageType = StorageType.Ssd,
                LocationAsLocationName = new LocationName(projectId, zone),
                ServeNodes = 3
            };
            // Initialize request argument(s).
            CreateInstanceRequest createInstanceRequest = new CreateInstanceRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
                Instance = myInstance,
                InstanceId = instanceId,
                Clusters = { { instanceId + "-c1", myCluster } }
            };

            try
            {
                // Make a request.
                Operation<Instance, CreateInstanceMetadata> createInstanceResponse =
                    bigtableInstanceAdminClient.CreateInstance(createInstanceRequest);
                Console.WriteLine("Waiting for operation to complete...");

                // Poll until the returned long-running operation is complete
                Operation<Instance, CreateInstanceMetadata> completedResponse =
                    createInstanceResponse.PollUntilCompleted();
                Console.WriteLine(
                    $"Instance: {displayName} {(completedResponse.IsCompleted ? "was successfully created in " : "failed to create in ")}{projectId} project");
                PrintInstanceInfo(completedResponse.Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating {displayName} instance");
                Console.WriteLine(ex.Message);
            }
            // [END Create_Ssd_Instance}
            return 0;
        }

        public static object CreateDevHddInstance(string projectId, string displayName)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START Create_Hdd_Instance]
            displayName += " HDD Dev";
            string instanceId = Regex.Replace(displayName, @"[^A-Za-z0-9_\.~]+", "-").ToLower();
            string zone = "us-east1-b";
            // The instance to create.
            Instance myInstance = new Instance
            {
                DisplayName = displayName,
                Type = Instance.Types.Type.Development
            };
            //The cluster to be created within the instance.
            Cluster myCluster = new Cluster
            {
                DefaultStorageType = StorageType.Hdd,
                LocationAsLocationName = new LocationName(projectId, zone)
            };
            // Initialize request argument(s).
            CreateInstanceRequest createInstanceRequest = new CreateInstanceRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
                Instance = myInstance,
                InstanceId = instanceId,
                Clusters = { { instanceId + "-c1", myCluster } }
            };

            try
            {
                // Make a request.
                Operation<Instance, CreateInstanceMetadata> createInstanceResponse =
                    bigtableInstanceAdminClient.CreateInstance(createInstanceRequest);
                Console.WriteLine("Waiting for operation to complete...");

                // Poll until the returned long-running operation is complete
                Operation<Instance, CreateInstanceMetadata> completedResponse =
                    createInstanceResponse.PollUntilCompleted();
                Console.WriteLine(
                    $"Instance: {displayName} {(completedResponse.IsCompleted ? "was successfully created in " : "failed to create in ")}{projectId} project");
                PrintInstanceInfo(completedResponse.Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating {displayName} instance");
                Console.WriteLine(ex.Message);
            }
            // [END Create_Ssd_Instance}
            return 0;
        }

        public static object CreateTwoClusterInstance(string projectId, string displayName)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START Create_Ssd_Instance]
            displayName += " Two Cluster";
            string instanceId = Regex.Replace(displayName, @"[^A-Za-z0-9_\.~]+", "-").ToLower();
            string zone1 = "us-east1-b";
            string zone2 = "us-east1-c";
            // The instance to create.
            Instance myInstance = new Instance
            {
                DisplayName = displayName,
                Type = Instance.Types.Type.Production
            };
            // The first cluster to be created within the instance.
            Cluster myCluster1 = new Cluster
            {
                DefaultStorageType = StorageType.Ssd,
                LocationAsLocationName = new LocationName(projectId, zone1),
                ServeNodes = 3
            };

            // The second cluster to be created within the instance.
            Cluster myCluster2 = new Cluster
            {
                DefaultStorageType = StorageType.Ssd,
                LocationAsLocationName = new LocationName(projectId, zone2),
                ServeNodes = 3
            };
            // Initialize request argument(s).
            CreateInstanceRequest createInstanceRequest = new CreateInstanceRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
                Instance = myInstance,
                InstanceId = instanceId,
                Clusters =
                {
                    {instanceId + "-c1", myCluster1},
                    {instanceId + "-c2", myCluster2}
                }
            };

            try
            {
                // Make a request.
                Operation<Instance, CreateInstanceMetadata> createInstanceResponse =
                    bigtableInstanceAdminClient.CreateInstance(createInstanceRequest);
                Console.WriteLine("Waiting for operation to complete...");

                // Poll until the returned long-running operation is complete
                Operation<Instance, CreateInstanceMetadata> completedResponse =
                    createInstanceResponse.PollUntilCompleted();
                Console.WriteLine(
                    $"Instance: {displayName} {(completedResponse.IsCompleted ? "was successfully created in " : "failed to create in ")}{projectId} project");
                PrintInstanceInfo(completedResponse.Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating {displayName} instance");
                Console.WriteLine(ex.Message);
            }
            // [END Create_Ssd_Instance}
            return 0;
        }

        public static object ListInstances(string projectId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START list_instances]
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
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"{"Instance Count:",-25}{instances.Instances.Count} instances in project {projectId}");
                foreach (Instance inst in instances.Instances)
                {
                    PrintInstanceInfo(inst);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while requesting information about instances in {projectId} project");
                Console.WriteLine(ex.Message);
                return -1;
            }
            Console.WriteLine(new string('-', 50));
            // [END list_instances]
            return 0;
        }

        public static object GetInstance(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START Get_instance]
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
                PrintInstanceInfo(respond);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception retreiving {instanceId} instance");
                Console.WriteLine(ex.Message);
            }
            return 0;
        }

        public static object PartialUpdateInstance(string projectId, string instaceId, string displayName = null,
            bool uppgradeToProd = false, MapField<string, string> labels = null)
        {
            return 0;
        }

        public static object UpdateInstanceDisplayName(string projectId, string instanceId, string newDisplayName)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // Print current instance information
            GetInstance(projectId, instanceId);

            // [START Update_InstanceDisplayName]
            // Create an instance object with new display name.
            Instance currentInstance = new Instance
            {
                InstanceName = new InstanceName(projectId, instanceId),
                DisplayName = newDisplayName
            };

            // Initialize request argument(s).
            PartialUpdateInstanceRequest partialUpdateInstanceRequest = new PartialUpdateInstanceRequest
            {
                UpdateMask = new FieldMask
                {
                    Paths =
                    {
                        "display_name"
                    }
                },
                Instance = currentInstance
            };
            try
            {
                // Make request
                Operation<Instance, UpdateInstanceMetadata> response = bigtableInstanceAdminClient.PartialUpdateInstance(partialUpdateInstanceRequest);
                Console.WriteLine("Waiting for operation to complete...");
                // Poll until the returned long-running operation is complete
                Operation<Instance, UpdateInstanceMetadata> completedResponse = response.PollUntilCompleted();
                // [END Update_InstanceDisplayName]
                // Print updated instance information.
                Console.WriteLine($"Printing updated instance information");
                GetInstance(projectId, instanceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception updating {instanceId} instance");
                Console.WriteLine(ex.Message);
            }

            return 0;
        }

        public static object UpgradeInstanceToProd(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // Print current instance information
            GetInstance(projectId, instanceId);

            // [START Update_InstanceDisplayName]
            // Create instance object with type changed to `PRODUCTION`.
            Instance currentInstance = new Instance
            {
                InstanceName = new InstanceName(projectId, instanceId),
                Type = Instance.Types.Type.Production
            };

            // Initialize request argument(s).
            PartialUpdateInstanceRequest partialUpdateInstanceRequest = new PartialUpdateInstanceRequest
            {
                UpdateMask = new FieldMask
                {
                    Paths =
                    {
                        "type"
                    }
                },
                Instance = currentInstance
            };
            try
            {
                // Make requesgt
                Operation<Instance, UpdateInstanceMetadata> response = bigtableInstanceAdminClient.PartialUpdateInstance(partialUpdateInstanceRequest);
                Console.WriteLine("Waiting for operation to complete...");
                // Poll until the returned long-running operation is complete
                Operation<Instance, UpdateInstanceMetadata> completedResponse = response.PollUntilCompleted();
                // [END Update_InstanceDisplayName]
                // Print updated instance information.
                PrintInstanceInfo(completedResponse.Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception upgrading {instanceId} instance");
                Console.WriteLine(ex.Message);
            }

            return 0;
        }

        public static object AddLabels(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // Print current instance information
            GetInstance(projectId, instanceId);

            // [START Add_Labels]
            // Create an instance object with label.
            Instance currentInstance = new Instance
            {
                InstanceName = new InstanceName(projectId, instanceId),
                Labels = {{"type", "dotnet"}, {"test", "bigtable"}}
            };

            // Initialize request argument(s).
            PartialUpdateInstanceRequest partialUpdateInstanceRequest = new PartialUpdateInstanceRequest
            {
                UpdateMask = new FieldMask
                {
                    Paths =
                    {
                        "labels"
                    }
                },
                Instance = currentInstance
            };
            try
            {
                // Make request
                Operation<Instance, UpdateInstanceMetadata> response = bigtableInstanceAdminClient.PartialUpdateInstance(partialUpdateInstanceRequest);
                Console.WriteLine("Waiting for operation to complete...");
                // Poll until the returned long-running operation is complete
                Operation<Instance, UpdateInstanceMetadata> completedResponse = response.PollUntilCompleted();
                // [END Update_InstanceDisplayName]
                // Print updated instance information.
                Console.WriteLine($"Printing updated instance information");
                GetInstance(projectId, instanceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception updating {instanceId} instance");
                Console.WriteLine(ex.Message);
            }

            return 0;
        }

        public static object UpdateInstanceMultipleFields(string projectId, string instanceId, string newDisplayName)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // Print current instance information
            GetInstance(projectId, instanceId);

            // [START Update_InstanceDisplayName]
            // Create an instance object with new display name.
            Instance currentInstance = new Instance
            {
                InstanceName = new InstanceName(projectId, instanceId),
                DisplayName = newDisplayName,
                Labels = {{"test", "updated displayName"}}
            };

            // Initialize request argument(s).
            PartialUpdateInstanceRequest partialUpdateInstanceRequest = new PartialUpdateInstanceRequest
            {
                UpdateMask = new FieldMask
                {
                    Paths =
                    {
                        "display_name",
                        "labels"
                    }
                },
                Instance = currentInstance
            };
            try
            {
                // Make request
                Operation<Instance, UpdateInstanceMetadata> response = bigtableInstanceAdminClient.PartialUpdateInstance(partialUpdateInstanceRequest);
                Console.WriteLine("Waiting for operation to complete...");
                // Poll until the returned long-running operation is complete
                Operation<Instance, UpdateInstanceMetadata> completedResponse = response.PollUntilCompleted();
                // [END Update_InstanceDisplayName]
                // Print updated instance information.
                Console.WriteLine($"Printing updated instance information");
                GetInstance(projectId, instanceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception updating {instanceId} instance");
                Console.WriteLine(ex.Message);
            }

            return 0;
        }

        public static object DeleteInstance(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START Delete_instance]
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
                Console.WriteLine($"Instance {instanceId} deleted successfuly");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while deleting {instanceId} instance");
                Console.WriteLine(ex.Message);
            }
            return 0;
        }

        public static object CreateCluster(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START Create_cluster]
            Cluster myCluster2 = new Cluster
            {
                DefaultStorageType = StorageType.Ssd,
                LocationAsLocationName = new LocationName(projectId, "us-east1-d"),
                ServeNodes = 3
            };
            // Initialize request argument(s).
            CreateClusterRequest request = new CreateClusterRequest
            {
                ParentAsInstanceName = new InstanceName(projectId, instanceId),
                ClusterId = instanceId + "-c3",
                Cluster = myCluster2
            };
            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                Operation<Cluster, CreateClusterMetadata> response = bigtableInstanceAdminClient.CreateCluster(request);
                // Poll until the returned long-running operation is complete
                Operation<Cluster, CreateClusterMetadata> completedResponse =
                    response.PollUntilCompleted();
                Console.WriteLine($"Cluster {request.ClusterId} was created successfully in instance {instanceId}");
                GetInstance(projectId, instanceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception creating additional cluster {request.ClusterId} in instance {instanceId}");
                Console.WriteLine(ex.Message);
            }
            // [END Craete_cluster]

            return 0;
        }

        public static object GetCluster(string projectId, string instanceId, string clusterId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START Get_Cluster]
            // Initialize request argument(s)
            GetClusterRequest getClusterRequest = new GetClusterRequest
            {
                ClusterName = new ClusterName(projectId, instanceId, clusterId)
            };

            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                Cluster response = bigtableInstanceAdminClient.GetCluster(getClusterRequest);
                PrintClusterInfo(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception getting cluster {clusterId} from instance {instanceId}");
                Console.WriteLine(ex.Message);
            }
            // [END Get_Cluster]
            return 0;
        }

        public static object ListAllClusters(string projectId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START list_Clusters]
            // Initialize request argument(s)
            ListClustersRequest listClustersRequest = new ListClustersRequest
            {
                ParentAsInstanceName = new InstanceName(projectId, "-")
            };

            try
            {
                // Make a request.
                Console.WriteLine("Waiting for operation to complete...");
                ListClustersResponse response = bigtableInstanceAdminClient.ListClusters(listClustersRequest);
                Console.WriteLine($"{"Cluster count:",-25}{response.Clusters.Count}");
                foreach (Cluster clstr in response.Clusters)
                {
                    PrintClusterInfo(clstr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while requesting information about clusters in {projectId} project");
                Console.WriteLine(ex.Message);
            }
            // [END list_Clusters]

            return 0;
        }
        
        public static object ListClusters(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START list_Clusters]
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
                Console.WriteLine($"{"Cluster count:",-25}{response.Clusters.Count}");
                foreach (Cluster clstr in response.Clusters)
                {
                    PrintClusterInfo(clstr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while requesting information about clusters in {instanceId} instance");
                Console.WriteLine(ex.Message);
            }
            // [END list_Clusters]

            return 0;
        }

        public static object UpdateClusterNodeCount(string projectId, string instanceId, string clusterId, int nodes)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START update_Cluster_Node_Count]
            // Initialize request argument(s)
            Cluster updatesCluster = new Cluster
            {
                ClusterName = new ClusterName(projectId, instanceId, clusterId),
                ServeNodes = nodes
            };

            Console.WriteLine($"Current {clusterId} cluster info");
            GetCluster(projectId, instanceId, clusterId);

            try
            {
                // Make the request to update cluster
                Console.WriteLine("Waiting for operation to complete...");
                Operation<Cluster, UpdateClusterMetadata> response = bigtableInstanceAdminClient.UpdateCluster(updatesCluster);

                // Poll until the returned long-running operation is complete
                Operation<Cluster, UpdateClusterMetadata> completedResponse = response.PollUntilCompleted();

                Console.WriteLine(
                    $"Cluster: {clusterId} {(completedResponse.IsCompleted ? "was successfully updated in " : "failed to update in ")}{instanceId} instance");

                Console.WriteLine($"Updated {clusterId} cluster info");
                GetCluster(projectId, instanceId, clusterId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while updating cluster {clusterId} in {instanceId} instance");
                Console.WriteLine(ex.Message);
            }
            // [END update_Cluster_Node_Count]
            return 0;
        }

        private static void PrintInstanceInfo(Instance instance)
        {
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Printing instance {instance.InstanceName.InstanceId}");
            Console.WriteLine(
                $"{"Instance ID:",-25}{instance.InstanceName.InstanceId}\n{"Instance Display Name:",-25}{instance.DisplayName}\n{"Type:",-25}{instance.Type}\n{"State:",-25}{instance.State}");
            if (instance.Labels.Any())
            {
                Console.WriteLine($"Printing instance {instance.InstanceName.InstanceId}");
                Console.WriteLine($"{"Label Count:",-25}{instance.Labels.Count}");
                foreach (string labelsKey in instance.Labels.Keys)
                {
                    Console.WriteLine($"{"{",26}{labelsKey} : {instance.Labels[labelsKey]}}}");
                }
            }
            ListClusters(instance.InstanceName.ProjectId, instance.InstanceName.InstanceId);
        }

        private static void PrintClusterInfo(Cluster cluster)
        {
            Console.WriteLine($"Printing cluster {cluster.ClusterName.ClusterId}");
            Console.WriteLine(
                $"{"  Cluster ID:",-25}{cluster.ClusterName.ClusterId}\n{"  Storage Type:",-25}{cluster.DefaultStorageType}" +
                $"\n{"  Location:",-25}{cluster.LocationAsLocationName.LocationId}\n{"  Node Count:",-25}{cluster.ServeNodes}\n{"  State:",-25}{cluster.State}\n");
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                    CreateProdSsdInstanceOptions, CreateDevHddInstanceOptions, 
                    CreateTwoClusterInstanceOptions, ListInstancesOptions,
                    GetInstanceOptions,
                    UpdateInstanceDisplayNameOptions, UpgradeInstanceToProdOptions,
                    AddLabelsOptions,
                    UpdateInstanceMultipleFieldsOptions,
                    DeleteInstanceOptions, CreateClusterOptions, GetClusterOptions,
                    ListAllClustersOptions,
                    ListClustersOptions, UpdateClusterNodeCountOptions>(args)
                .MapResult(
                    (CreateProdSsdInstanceOptions opts) => CreateProdSsdInstance(opts.projectId, opts.displayName),
                    (CreateDevHddInstanceOptions opts) => CreateDevHddInstance(opts.projectId, opts.displayName),
                    (CreateTwoClusterInstanceOptions opts) => CreateTwoClusterInstance(opts.projectId, opts.displayName),
                    (ListInstancesOptions opts) => ListInstances(opts.projectId),
                    (GetInstanceOptions opts) => GetInstance(opts.projectId, opts.instanceId),
                    (UpdateInstanceDisplayNameOptions opts) => UpdateInstanceDisplayName(opts.projectId, opts.instanceId, opts.newDisplayName),
                    (UpgradeInstanceToProdOptions opts) => UpgradeInstanceToProd(opts.projectId, opts.instanceId),
                    (AddLabelsOptions opts) => AddLabels(opts.projectId, opts.instanceId),
                    (UpdateInstanceMultipleFieldsOptions opts) => UpdateInstanceMultipleFields(opts.projectId, opts.instanceId, opts.newDisplayName),
                    (DeleteInstanceOptions opts) => DeleteInstance(opts.projectId, opts.instanceId),
                    (CreateClusterOptions opts) => CreateCluster(opts.projectId, opts.instanceId),
                    (GetClusterOptions opts) => GetCluster(opts.projectId, opts.instanceId, opts.clusterId),
                    (ListAllClustersOptions opts) => ListAllClusters(opts.projectId),
                    (ListClustersOptions opts) => ListClusters(opts.projectId, opts.instanceId),
                    (UpdateClusterNodeCountOptions opts) =>
                        UpdateClusterNodeCount(opts.projectId, opts.instanceId, opts.clusterId, opts.nodes), errs => 1);
        }
    }
}
