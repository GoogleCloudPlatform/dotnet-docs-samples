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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Gaming.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

public class CreateConfigSample
{
    public async Task<GameServerConfig> CreateConfig(
        string projectId, string regionId, string deploymentId, string configId)
    {
        // Create the client.
        GameServerConfigsServiceClient client = await GameServerConfigsServiceClient.CreateAsync();

        FleetConfig fleetConfig = new FleetConfig
        {
            Name = "my-fleet-spec",
            FleetSpec = "{\"replicas\":10,\"scheduling\":\"Packed\",\"strategy\":{\"type\":\"RollingUpdate\",\"rollingUpdate\":{\"maxSurge\":\"25%\",\"maxUnavailable\":\"25%\"}},\"template\":{\"metadata\":{\"labels\":{\"gameName\":\"udp-server\"}},\"spec\":{\"ports\":[{\"name\":\"default\",\"portPolicy\":\"Dynamic\",\"containerPort\":2156,\"protocol\":\"TCP\"}],\"health\":{\"initialDelaySeconds\":30,\"periodSeconds\":60},\"sdkServer\":{\"logLevel\":\"Info\",\"grpcPort\":9357,\"httpPort\":9358},\"template\":{\"spec\":{\"containers\":[{\"name\":\"dedicated\",\"image\":\"gcr.io/agones-images/udp-server:0.17\",\"imagePullPolicy\":\"Always\",\"resources\":{\"requests\":{\"memory\":\"200Mi\",\"cpu\":\"500m\"},\"limits\":{\"memory\":\"200Mi\",\"cpu\":\"500m\"}}}]}}}}}"
        };

        GameServerConfig config = new GameServerConfig
        {
            GameServerConfigName = GameServerConfigName.FromProjectLocationDeploymentConfig(projectId, regionId, deploymentId, configId),
            Description = "My Game Server Config"
        };
        config.FleetConfigs.Add(fleetConfig);

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