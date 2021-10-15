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

// [START cloud_game_servers_rollout_get]

using Google.Cloud.Gaming.V1;

public class GetRolloutSample
{
    public GameServerDeploymentRollout GetRollout(
        string projectId, string deploymentId)
    {
        // Create the client.
        GameServerDeploymentsServiceClient client = GameServerDeploymentsServiceClient.Create();

        GetGameServerDeploymentRolloutRequest request = new GetGameServerDeploymentRolloutRequest
        {
            GameServerDeploymentName = GameServerDeploymentName.FromProjectLocationDeployment(projectId, "global", deploymentId)
        };

        // Make the request.
        GameServerDeploymentRollout response = client.GetGameServerDeploymentRollout(request);
        return response;
    }
}
// [END cloud_game_servers_rollout_get]