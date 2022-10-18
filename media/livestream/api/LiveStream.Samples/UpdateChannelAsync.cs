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

// [START livestream_update_channel]

using Google.Cloud.Video.LiveStream.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

public class UpdateChannelSample
{
    public async Task<Channel> UpdateChannelAsync(
         string projectId, string locationId, string channelId, string inputId)
    {
        // Create the client.
        LivestreamServiceClient client = LivestreamServiceClient.Create();

        InputAttachment inputAttachment = new InputAttachment
        {
            Key = "updated-input",
            InputAsInputName = InputName.FromProjectLocationInput(projectId, locationId, inputId)
        };

        UpdateChannelRequest request = new UpdateChannelRequest
        {
            Channel = new Channel
            {
                ChannelName = ChannelName.FromProjectLocationChannel(projectId, locationId, channelId),
                InputAttachments = { inputAttachment }
            },
            UpdateMask = new FieldMask { Paths = { "input_attachments" } }
        };

        // Make the request.
        Operation<Channel, OperationMetadata> response = await client.UpdateChannelAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<Channel, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END livestream_update_channel]