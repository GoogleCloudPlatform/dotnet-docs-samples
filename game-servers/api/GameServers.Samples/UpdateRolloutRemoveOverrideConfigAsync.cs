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

// [START cloud_game_servers_deployment_rollout_remove_override]

using Google.Cloud.Gaming.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

public class UpdateRolloutRemoveOverrideConfigSample
{
    public async Task<GameServerDeployment> UpdateRolloutRemoveOverrideConfigAsync(
        string projectId, string deploymentId)
    {
        // Create the client.
        GameServerDeploymentsServiceClient client = await GameServerDeploymentsServiceClient.CreateAsync();

        GameServerDeploymentRollout rollout = new GameServerDeploymentRollout
        {
            Name = GameServerDeploymentName.FormatProjectLocationDeployment(projectId, "global", deploymentId)
        };

        UpdateGameServerDeploymentRolloutRequest request = new UpdateGameServerDeploymentRolloutRequest
        {
            Rollout = rollout,
            UpdateMask = new FieldMask { Paths = { "game_server_config_overrides" } }
        };

        // Make the request.
        Operation<GameServerDeployment, OperationMetadata> response = await client.UpdateGameServerDeploymentRolloutAsync(request);
        Operation<GameServerDeployment, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END cloud_game_servers_deployment_rollout_remove_override]