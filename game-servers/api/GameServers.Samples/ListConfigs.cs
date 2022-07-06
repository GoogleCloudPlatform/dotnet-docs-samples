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

// [START cloud_game_servers_config_list]

using Google.Api.Gax;
using Google.Cloud.Gaming.V1;
using System.Collections.Generic;
using System.Linq;

public class ListConfigsSample
{
    public IList<GameServerConfig> ListConfigs(
        string projectId, string regionId, string deploymentId)
    {
        // Create the client.
        GameServerConfigsServiceClient client = GameServerConfigsServiceClient.Create();

        ListGameServerConfigsRequest request = new ListGameServerConfigsRequest
        {
            ParentAsGameServerDeploymentName = GameServerDeploymentName.FromProjectLocationDeployment(projectId, regionId, deploymentId)
        };

        // Make the request.
        PagedEnumerable<ListGameServerConfigsResponse, GameServerConfig> response = client.ListGameServerConfigs(request);

        // The returned sequence will lazily perform RPCs as it's being iterated over.
        return response.ToList();
    }
}
// [END cloud_game_servers_config_list]