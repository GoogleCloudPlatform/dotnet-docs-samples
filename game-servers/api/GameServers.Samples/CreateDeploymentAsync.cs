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

// [START cloud_game_servers_deployment_create]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Gaming.V1;
using Google.LongRunning;
using System.Threading.Tasks;

public class CreateDeploymentSample
{
    public async Task<GameServerDeployment> CreateDeploymentAsync(
        string projectId, string deploymentId)
    {
        // Create the client.
        GameServerDeploymentsServiceClient client = await GameServerDeploymentsServiceClient.CreateAsync();

        GameServerDeployment deployment = new GameServerDeployment()
        {
            GameServerDeploymentName = GameServerDeploymentName.FromProjectLocationDeployment(projectId, "global", deploymentId)
        };
        CreateGameServerDeploymentRequest request = new CreateGameServerDeploymentRequest
        {
            DeploymentId = deploymentId,
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, "global"),
            GameServerDeployment = deployment
        };

        // Make the request.
        Operation<GameServerDeployment, OperationMetadata> response = await client.CreateGameServerDeploymentAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<GameServerDeployment, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END cloud_game_servers_deployment_create]