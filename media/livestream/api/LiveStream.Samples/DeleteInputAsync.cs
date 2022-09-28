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

// [START livestream_delete_input]

using Google.Cloud.Video.LiveStream.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

public class DeleteInputSample
{
    public async Task DeleteInputAsync(
         string projectId, string locationId, string inputId)
    {
        // Create the client.
        LivestreamServiceClient client = LivestreamServiceClient.Create();

        DeleteInputRequest request = new DeleteInputRequest
        {
            InputName = InputName.FromProjectLocationInput(projectId, locationId, inputId)
        };

        // Make the request.
        Operation<Empty, OperationMetadata> response = await client.DeleteInputAsync(request);

        // Poll until the returned long-running operation is complete.
        await response.PollUntilCompletedAsync();
    }
}
// [END livestream_delete_input]