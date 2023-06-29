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

// [START videostitcher_create_slate]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Stitcher.V1;
using Google.LongRunning;
using System.Threading.Tasks;

public class CreateSlateSample
{
    public async Task<Slate> CreateSlateAsync(
        string projectId, string location, string slateId, string slateUri)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        CreateSlateRequest request = new CreateSlateRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, location),
            SlateId = slateId,
            Slate = new Slate
            {
                Uri = slateUri
            }
        };

        // Make the request.
        Operation<Slate, OperationMetadata> response = await client.CreateSlateAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<Slate, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END videostitcher_create_slate]
