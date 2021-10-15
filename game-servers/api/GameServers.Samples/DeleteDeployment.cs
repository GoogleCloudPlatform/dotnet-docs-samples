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

// [START cloud_game_servers_deployment_delete]

using Google.Cloud.Gaming.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;

public class DeleteDeploymentSample
{
    public void DeleteDeployment(
        string projectId, string deploymentId)
    {
        // Create the client.
        GameServerDeploymentsServiceClient client = GameServerDeploymentsServiceClient.Create();

        DeleteGameServerDeploymentRequest request = new DeleteGameServerDeploymentRequest
        {
            GameServerDeploymentName = GameServerDeploymentName.FromProjectLocationDeployment(projectId, "global", deploymentId)
        };

        // Make the request.
        Operation<Empty, OperationMetadata> response = client.DeleteGameServerDeployment(request);

        // Poll until the returned long-running operation is complete.
        response.PollUntilCompleted();
    }
}
// [END cloud_game_servers_deployment_delete]