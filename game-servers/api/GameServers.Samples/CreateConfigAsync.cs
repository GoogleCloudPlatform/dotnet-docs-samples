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

// [START cloud_game_servers_config_create]

using Google.Cloud.Gaming.V1;
using Google.LongRunning;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class CreateConfigSample
{
    public async Task<GameServerConfig> CreateConfigAsync(
        string projectId, string regionId, string deploymentId, string configId)
    {
        // Create the client.
        GameServerConfigsServiceClient client = await GameServerConfigsServiceClient.CreateAsync();

        GameServerConfig config = new GameServerConfig
        {
            GameServerConfigName = GameServerConfigName.FromProjectLocationDeploymentConfig(projectId, regionId, deploymentId, configId),
            Description = "My Game Server Config",
            FleetConfigs =
            {
                new FleetConfig
                {

                    Name = "fleet-spec-1",
                    FleetSpec = JsonConvert.SerializeObject(new
                    {
                        replicas = 10,
                        scheduling = "Packed",
                        strategy = new
                        {
                            type = "RollingUpdate",
                            rollingUpdate = new
                            {
                                maxSurge = "25%",
                                maxUnavailable = "25%",
                            }
                        },
                        template = new
                        {
                            metadata = new
                            {
                                labels = new
                                {
                                    gameName = "udp-server",
                                }
                            },
                            spec = new
                            {
                                ports = new [] {
                                    new {
                                        name = "default",
                                        portPolicy = "Dynamic",
                                        containerPort = 7654,
                                        protocol = "UDP",
                                    }
                                },
                                health = new
                                {
                                    initialDelaySeconds = 30,
                                    periodSeconds = 60,
                                },
                                sdkServer = new
                                {
                                    logLevel = "Info",
                                    grpcPort = 9357,
                                    httpPort = 9358,
                                },
                                template = new
                                {
                                    spec = new
                                    {
                                        containers = new [] {
                                            new {
                                                name = "dedicated",
                                                image = "gcr.io/agones-images/udp-server:0.21",
                                                imagePullPolicy = "Always",
                                                resources = new
                                                {
                                                    requests = new
                                                    {
                                                        memory = "200Mi",
                                                        cpu = "500m",
                                                    },
                                                    limits = new
                                                    {
                                                        memory = "200Mi",
                                                        cpu = "500m",
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    })
                }
            }
        };

        CreateGameServerConfigRequest request = new CreateGameServerConfigRequest
        {
            ParentAsGameServerDeploymentName = GameServerDeploymentName.FromProjectLocationDeployment(projectId, regionId, deploymentId),
            ConfigId = configId,
            GameServerConfig = config
        };

        // Make the request.
        Operation<GameServerConfig, OperationMetadata> response = await client.CreateGameServerConfigAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<GameServerConfig, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END cloud_game_servers_config_create]