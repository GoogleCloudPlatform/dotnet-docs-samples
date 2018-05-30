using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;
using Google.Api.Gax;
using Google.LongRunning;
using Google.Protobuf.Collections;
using Google.Cloud.Bigtable.V2;
using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Iam.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace GoogleCloudSamples.Bigtable
{
    [Verb("createProdSsdInstance", HelpText = "Create a `PRODUCTION` type Instance with SSD storage type in this project.")]
    class CreateProdSsdInstanceOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The displayName for an instance to create.", Required = true)]
        public string displayName { get; set; }
    }

    [Verb("createDevHddInstance", HelpText = "Create a `DEVELOPMENT` type Instance with HDD storage type in this project.")]
    class CreateDevHddInstanceOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The displayName for an instance to create.", Required = true)]
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

    [Verb("updateInstanceMultipleFields", HelpText = "Change a display name and replace labels of an existing instance in one request")]
    class UpdateInstanceMultipleFieldsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to be updated.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The new displayName for the instance.", Required = false)]
        public string newDisplayName { get; set; }
    }

    [Verb("addLabels", HelpText = "Try to add a label to an instance that may contain labels already")]
    class AddLabelsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to be updated.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("updateLabels", HelpText = "Update value of an existing label or add a new label if doesn't exist already in an instance")]
    class UpdateLabelsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to be updated.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("deleteLabels", HelpText = "Update value of an existing label in an instance")]
    class DeleteLabelsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to delete labels.", Required = true)]
        public string instanceId { get; set; }
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

    [Verb("deleteCluster", HelpText = "Delete a cluater from an instance.")]
    class DeleteClusterOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId to which cluster belongs.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("createAppProfile", HelpText = "Create an AppProfile in an instance")]
    class CreateAppProfileOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to use.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("getAppProfile", HelpText = "Getrs information about an AppProfile in an instance")]
    class GetAppProfileOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to use.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("listAppProfiles", HelpText = "List information about AppProfiles in an instance")]
    class ListAppProfilesOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to use.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("updateAppProfile", HelpText = "Update an AppProfile in an instance")]
    class UpdateAppProfileOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to use.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("deleteAppProfile", HelpText = "Delete an AppProfile from an instance")]
    class DeleteAppProfileOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to use.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("getInstanceIamPolicy", HelpText = "Gets information about Policy")]
    class GetInstancetIamPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to use.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("setInstanceIamPolicy", HelpText = "Sets Policy")]
    class SetInstanceIamPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to use.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("testIamPermissions", HelpText = "Returns permissions that a caller has on the instance")]
    class TestIamPermissionsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for bigtable operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The instanceId of the instance to use.", Required = true)]
        public string instanceId { get; set; }
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
                Console.WriteLine($"{"Instance Count:",-30}{instances.Instances.Count} instances in project {projectId}");
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
                //DisplayName = newDisplayName,
                Labels = { {"type", "updated_name"}}
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
        
        public static object AddLabels(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // Print current instance information
            GetInstance(projectId, instanceId);

            // [START Add_Labels]
            // Get an instance object which labels need to be updated.
            GetInstanceRequest request = new GetInstanceRequest
            {
                InstanceName = new InstanceName(projectId, instanceId)
            };
            Instance currentInstance = bigtableInstanceAdminClient.GetInstance(request);

            // TODO: Instead of try-catch block below I may add labels in the following way?
            /*
             * The following will add a label if key doesn't exist or update a value of an exisitng key.
             * currentInstance.Labels["type"] = "dotnet";
             * currentInstance.Labels["test"] = "bigtable";             *
             */
            // Add labels.
            try
            {
                currentInstance.Labels.Add(new MapField<string, string> {{"type", "dotnet"}, {"test", "bigtable"}});
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception adding labels");
                Console.WriteLine(ex.Message);
                return 0;
            }
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

        public static object UpdateLabels(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // Print current instance information
            GetInstance(projectId, instanceId);

            string labelKey = "test";
            string labelNewValue = "bigtable-example";
            // [START Add_Labels]
            // Get an instance object which labels need to be updated.
            GetInstanceRequest request = new GetInstanceRequest
            {
                InstanceName = new InstanceName(projectId, instanceId)
            };
            Instance currentInstance = bigtableInstanceAdminClient.GetInstance(request);

            // Update labels in the current instance
            currentInstance.Labels[labelKey] = labelNewValue;

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

        public static object DeleteLabels(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // Print current instance information
            GetInstance(projectId, instanceId);

            // [START Add_Labels]
            // Create an instance object with an empty fiels labels.
            Instance currentInstance = new Instance
            {
                InstanceName = new InstanceName(projectId, instanceId),
                Labels = { }
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

            // Print current instance information
            GetInstance(projectId, instanceId);

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
                ClusterId = instanceId + "-c2",
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
                Console.WriteLine($"{"Cluster count:",-30}{response.Clusters.Count}");
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
                Console.WriteLine($"{"Cluster count:",-30}{response.Clusters.Count}");
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

            Console.WriteLine($"Current {clusterId} cluster info");
            GetCluster(projectId, instanceId, clusterId);
            // [START update_Cluster_Node_Count]
            // Initialize request argument(s)
            Cluster updatedCluster = new Cluster
            {
                ClusterName = new ClusterName(projectId, instanceId, clusterId),
                ServeNodes = nodes
            };

            try
            {
                // Make the request to update cluster
                Console.WriteLine("Waiting for operation to complete...");
                Operation<Cluster, UpdateClusterMetadata> response = bigtableInstanceAdminClient.UpdateCluster(updatedCluster);

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

        public static object DeleteCluster(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]


            // Print current instance information
            GetInstance(projectId, instanceId);

            // [START delete_Cluster]
            // Initialize request argument(s)
            DeleteClusterRequest request = new DeleteClusterRequest
            {
                ClusterName = new ClusterName(projectId, instanceId, instanceId + "-c2")
            };
            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                bigtableInstanceAdminClient.DeleteCluster(request);
                Console.WriteLine($"Cluster {request.ClusterName.ClusterId} was deleted successfully from instance {instanceId}");
                GetInstance(projectId, instanceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception deleting cluster {request.ClusterName.ClusterId} from instance {instanceId}");
                Console.WriteLine(ex.Message);
            }
            // [END delete_cluster]
            return 0;
        }

        public static object CreateAppProfile(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START create_AppPrifile]
            // Create an AppProfile object
            AppProfile testAppProfile = new AppProfile
            {
                Description = "some desription",
                SingleClusterRouting = new AppProfile.Types.SingleClusterRouting
                {
                    ClusterId = instanceId + "-c1",
                    AllowTransactionalWrites = true
                }
            };
            // Initialize request argument(s)
            CreateAppProfileRequest request = new CreateAppProfileRequest
            { 
                ParentAsInstanceName = new InstanceName(projectId, instanceId),
                AppProfile = testAppProfile,
                AppProfileId = "my-test-AppProfile" 
            };
            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                AppProfile response = bigtableInstanceAdminClient.CreateAppProfile(request);

                testAppProfile = bigtableInstanceAdminClient.GetAppProfile(new AppProfileName(projectId, instanceId, "my-test-AppProfile"));
                PrintAppProfileInfo(testAppProfile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception creating appProfile {testAppProfile.Name} instance {instanceId}");
                Console.WriteLine(ex.Message);
            }
            // [END create_AppProfile]

            return 0;
        }

        public static object GetAppProfile(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START create_AppPrifile]
            // Initialize request argument(s)
            GetAppProfileRequest request = new GetAppProfileRequest
            {
                AppProfileName = new AppProfileName(projectId, instanceId, "my-test-AppProfile")
            };
            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                AppProfile response = bigtableInstanceAdminClient.GetAppProfile(request);

                PrintAppProfileInfo(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception retreaving appProfile {request.AppProfileName.AppProfileId} from instance {instanceId}");
                Console.WriteLine(ex.Message);
            }
            // [END createAppProfile]

            return 0;
        }

        public static object ListAppProfiles(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START create_AppPrifile]
            // Initialize request argument(s)
            ListAppProfilesRequest request = new ListAppProfilesRequest
            {
                ParentAsInstanceName = new InstanceName(projectId, instanceId)
            };
            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                PagedEnumerable<ListAppProfilesResponse, AppProfile> response = bigtableInstanceAdminClient.ListAppProfiles(request);
                var approfiles = response.Select(x => x).ToList();
                Console.WriteLine($"{"Approfiles count:",-30}{approfiles.Count}");
                foreach (AppProfile appProfile in approfiles)
                {
                    PrintAppProfileInfo(appProfile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while requesting information about appProfiles on instance {instanceId}");
                Console.WriteLine(ex.Message);
            }
            // [END delete_cluster]

            return 0;
        }

        public static object UpdateAppProfile(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START create_AppPrifile]
            // Create an AppProfile object
            GetAppProfileRequest getRequest = new GetAppProfileRequest
            {
                AppProfileName = new AppProfileName(projectId, instanceId, "my-test-AppProfile")
            };

            AppProfile updatedAppProfile = bigtableInstanceAdminClient.GetAppProfile(getRequest);
            // update properties of the current AppProfile
            updatedAppProfile.Description = "some updated desription";
            //updatedAppProfile.SingleClusterRouting.ClusterId = "";
            //updatedAppProfile.SingleClusterRouting.AllowTransactionalWrites = false;
            updatedAppProfile.ClearRoutingPolicy();
            updatedAppProfile.MultiClusterRoutingUseAny = new AppProfile.Types.MultiClusterRoutingUseAny();
            // Initialize request argument(s)
            UpdateAppProfileRequest request = new UpdateAppProfileRequest
            {
                UpdateMask = new FieldMask
                {
                    Paths =
                    {
                        "description",
                        "multi_cluster_routing_use_any",
                        //"single_cluster_routing",

                    }
                },
                AppProfile = updatedAppProfile,
                IgnoreWarnings = true
            };
            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                Operation<AppProfile, UpdateAppProfileMetadata> response = bigtableInstanceAdminClient.UpdateAppProfile(request);
                // Poll until the returned long-running operation is complete
                Operation<AppProfile, UpdateAppProfileMetadata> completedResponse =
                    response.PollUntilCompleted();
                // Print an updated AppProfile information
                GetAppProfile(projectId, instanceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception updating appProfile on instance {instanceId}");
                Console.WriteLine(ex.Message);
            }
            // [END create_AppProfile]

            return 0;
        }

        public static object DeleteAppProfile(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START create_AppPrifile]
            // Initialize request argument(s)
            DeleteAppProfileRequest request = new DeleteAppProfileRequest
            {
                AppProfileName = new AppProfileName(projectId, instanceId, "my-test-AppProfile")
            };
            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                bigtableInstanceAdminClient.DeleteAppProfile(request);

                ListAppProfiles(projectId, instanceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception deleting appProfile {request.AppProfileName.AppProfileId} from instance {instanceId}");
                Console.WriteLine(ex.Message);
            }
            // [END create_AppProfile]

            return 0;
        }

        public static object GetProjectIamPolicy(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START get_IamPolicy]
            // Initialize request argument(s)
            GetIamPolicyRequest request = new GetIamPolicyRequest
            {
                Resource = new InstanceName(projectId, instanceId).ToString()
            };

            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                Policy response = bigtableInstanceAdminClient.GetIamPolicy(request);

                PrintPolicyInfo(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception retreiving IamPolicy from instance");
                Console.WriteLine(ex.Message);
            }

            return 0;
        }

        public static object SetInstanceIamPolicy(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            // [START set_IamPolicy]
            // Initialize request argument(s)
            SetIamPolicyRequest request = new SetIamPolicyRequest
            {
                Resource = new InstanceName(projectId, instanceId).ToString(),
                Policy = new Policy
                { 
                    Bindings = { new Binding
                    {
                        Members = { "test-member1", "test-member2"},
                        Role = "bigtable.user"
                    }},
                    Etag = ByteString.CopyFromUtf8("test-etag"),
                    Version = 1
                }
            };
            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                Policy output = bigtableInstanceAdminClient.SetIamPolicy(request);
                //PrintPolicyInfo(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception setting IamPolicy");
                Console.WriteLine(ex.Message);
            }

            return 0;
        }

        public static object TestIamPermissions(string projectId, string instanceId)
        {
            // [START create_bigtableInstanceAdminClient]
            BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();
            // [END create_bigtableInstanceAdminClient]

            var permissions = new List<string>
            {
                "bigtable.tables.create",
                "bigtable.clusters.create",
                "bigtable.instances.update",
                "bigtable.instances.setIamPolicy",
            };

            // [START test_IamPermissions]
            // Initialize request argument(s)
            TestIamPermissionsRequest request = new TestIamPermissionsRequest
            {
                Resource = new InstanceName(projectId, instanceId).ToString(),
                Permissions = { permissions }
            };

            Console.WriteLine("Testing the following permissions");
            PrintPermissionsInfo(permissions);
            Console.WriteLine(new string('-', 20));

            try
            {
                // Make the request
                Console.WriteLine("Waiting for operation to complete...");
                TestIamPermissionsResponse response = bigtableInstanceAdminClient.TestIamPermissions(request);

                Console.WriteLine("These permissions are granted");
                PrintPermissionsInfo(response.Permissions);
                Console.WriteLine(new string('-', 20));

                var notGranted = permissions.Except(response.Permissions).ToList();
                Console.WriteLine("These permissions are missing");
                PrintPermissionsInfo(notGranted);
                Console.WriteLine(new string('-', 20));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception retreiving IamPolicy from instance");
                Console.WriteLine(ex.Message);
            }

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
            ListClusters(instance.InstanceName.ProjectId, instance.InstanceName.InstanceId);
        }

        private static void PrintClusterInfo(Cluster cluster)
        {
            Console.WriteLine($"Printing cluster {cluster.ClusterName.ClusterId}");
            Console.WriteLine(
                $"{"  Cluster ID:",-30}{cluster.ClusterName.ClusterId}\n{"  Storage Type:",-30}{cluster.DefaultStorageType}" +
                $"\n{"  Location:",-30}{cluster.LocationAsLocationName.LocationId}\n{"  Node Count:",-30}{cluster.ServeNodes}\n{"  State:",-30}{cluster.State}\n");
        }

        private static void PrintAppProfileInfo(AppProfile approfile)
        {
            Console.WriteLine($"Printing approfile {approfile.Name.Split('/').Last()}");
            Console.WriteLine(
                $"{"Approfile ID:",-30}{approfile.Name.Split('/').Last()}\n{"  Description:",-30}{approfile.Description}" +
                $"\n{"  RoutingPolicy:",-30}{approfile.RoutingPolicyCase}");
            if (approfile.RoutingPolicyCase == AppProfile.RoutingPolicyOneofCase.SingleClusterRouting)
            {
                Console.WriteLine($"{"  ClusterId:",-30}{approfile.SingleClusterRouting.ClusterId}" +
                                  $"\n{"  AllowTransactionalWrites:",-30}{approfile.SingleClusterRouting.AllowTransactionalWrites}");
            }
        }

        private static void PrintPolicyInfo(Policy policy)
        {
            Console.WriteLine($"Printing policy");

            foreach (Binding policyBinding in policy.Bindings)
            {
                Console.WriteLine(
                    $"{"  Role:",-30}{policyBinding.Role}");

                foreach (string policyBindingMember in policyBinding.Members)
                {
                    Console.WriteLine($"{"  Member:",-30}{policyBindingMember}");
                }
            }
            Console.WriteLine(
                $"{"  Etag:",-30}{policy.Etag.ToBase64()}\n{"  Version:",-30}{policy.Version}");
        }

        private static void PrintPermissionsInfo(IEnumerable<string> permisisons)
        {
            foreach (string permission in permisisons)
            {
                Console.WriteLine($"  {permission}");
            }
        }

        static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            verbMap
                .Add((CreateProdSsdInstanceOptions opts) => CreateProdSsdInstance(opts.projectId, opts.displayName))
                .Add((CreateDevHddInstanceOptions opts) => CreateDevHddInstance(opts.projectId, opts.displayName))
                .Add((CreateTwoClusterInstanceOptions opts) => CreateTwoClusterInstance(opts.projectId, opts.displayName))
                .Add((ListInstancesOptions opts) => ListInstances(opts.projectId))
                .Add((GetInstanceOptions opts) => GetInstance(opts.projectId, opts.instanceId))
                .Add((UpdateInstanceDisplayNameOptions opts) => UpdateInstanceDisplayName(opts.projectId, opts.instanceId, opts.newDisplayName))
                .Add((UpgradeInstanceToProdOptions opts) => UpgradeInstanceToProd(opts.projectId, opts.instanceId))
                .Add((AddLabelsOptions opts) => AddLabels(opts.projectId, opts.instanceId))
                .Add((UpdateLabelsOptions opts) => UpdateLabels(opts.projectId, opts.instanceId))
                .Add((DeleteLabelsOptions opts) => DeleteLabels(opts.projectId, opts.instanceId))
                .Add((UpdateInstanceMultipleFieldsOptions opts) => UpdateInstanceMultipleFields(opts.projectId, opts.instanceId, opts.newDisplayName))
                .Add((DeleteInstanceOptions opts) => DeleteInstance(opts.projectId, opts.instanceId))
                .Add((CreateClusterOptions opts) => CreateCluster(opts.projectId, opts.instanceId))
                .Add((GetClusterOptions opts) => GetCluster(opts.projectId, opts.instanceId, opts.clusterId))
                .Add((ListAllClustersOptions opts) => ListAllClusters(opts.projectId))
                .Add((ListClustersOptions opts) => ListClusters(opts.projectId, opts.instanceId))
                .Add((UpdateClusterNodeCountOptions opts) => UpdateClusterNodeCount(opts.projectId, opts.instanceId, opts.clusterId, opts.nodes))
                .Add((DeleteClusterOptions opts) => DeleteCluster(opts.projectId, opts.instanceId))
                .Add((CreateAppProfileOptions opts) => CreateAppProfile(opts.projectId, opts.instanceId))
                .Add((GetAppProfileOptions opts) => GetAppProfile(opts.projectId, opts.instanceId))
                .Add((ListAppProfilesOptions opts) => ListAppProfiles(opts.projectId, opts.instanceId))
                .Add((UpdateAppProfileOptions opts) => UpdateAppProfile(opts.projectId, opts.instanceId))
                .Add((DeleteAppProfileOptions opts) => DeleteAppProfile(opts.projectId, opts.instanceId))
                .Add((GetInstancetIamPolicyOptions opts) => GetProjectIamPolicy(opts.projectId, opts.instanceId))
                .Add((SetInstanceIamPolicyOptions opts) => SetInstanceIamPolicy(opts.projectId, opts.instanceId))
                .Add((TestIamPermissionsOptions opts) => TestIamPermissions(opts.projectId, opts.instanceId))
                .NotParsedFunc = (err) => 1;
            return (int)verbMap.Run(args);

            // Can not use CommanLine.parser as it can only handle up to 16 objects
            #region CommandLine.parser
            /*
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
            */
            #endregion
        }
    }
}
