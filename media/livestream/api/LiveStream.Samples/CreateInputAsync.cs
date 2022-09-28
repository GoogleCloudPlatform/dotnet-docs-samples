/*
 * Copyright 2022 Google LLC
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

// [START livestream_create_input]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.LiveStream.V1;
using Google.LongRunning;
using System.Threading.Tasks;

public class CreateInputSample
{
    public async Task<Input> CreateInputAsync(
         string projectId, string locationId, string inputId)
    {
        // Create the client.
        LivestreamServiceClient client = LivestreamServiceClient.Create();

        CreateInputRequest request = new CreateInputRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, locationId),
            InputId = inputId,
            Input = new Input
            {
                Type = Input.Types.Type.RtmpPush
            }
        };

        // Make the request.
        Operation<Input, OperationMetadata> response = await client.CreateInputAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<Input, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END livestream_create_input]