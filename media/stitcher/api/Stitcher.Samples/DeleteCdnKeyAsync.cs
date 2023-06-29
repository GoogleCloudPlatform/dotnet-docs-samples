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

// [START videostitcher_delete_cdn_key]

using Google.Cloud.Video.Stitcher.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

public class DeleteCdnKeySample
{
    public async Task DeleteCdnKeyAsync(
        string projectId, string location, string cdnKeyId)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        DeleteCdnKeyRequest request = new DeleteCdnKeyRequest
        {
            CdnKeyName = CdnKeyName.FromProjectLocationCdnKey(projectId, location, cdnKeyId)
        };

        // Make the request.
        Operation<Empty, OperationMetadata> response = await client.DeleteCdnKeyAsync(request);

        // Poll until the returned long-running operation is complete.
        await response.PollUntilCompletedAsync();
    }
}
// [END videostitcher_delete_cdn_key]