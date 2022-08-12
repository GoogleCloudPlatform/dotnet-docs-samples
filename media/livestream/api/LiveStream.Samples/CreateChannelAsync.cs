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

// [START livestream_create_channel]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.LiveStream.V1;
using Google.LongRunning;
using System.Threading.Tasks;

public class CreateChannelSample
{
    public async Task<Channel> CreateChannelAsync(
         string projectId, string locationId, string channelId, string inputId, string outputUri)
    {
        // Create the client.
        LivestreamServiceClient client = LivestreamServiceClient.Create();

        InputAttachment inputAttachment = new InputAttachment
        {
            Key = "my-input",
            InputAsInputName = InputName.FromProjectLocationInput(projectId, locationId, inputId)
        };

        VideoStream videoStream = new VideoStream
        {
            H264 = new VideoStream.Types.H264CodecSettings
            {
                Profile = "high",
                BitrateBps = 3000000,
                FrameRate = 30,
                HeightPixels = 720,
                WidthPixels = 1280
            }
        };

        ElementaryStream elementaryStreamVideo = new ElementaryStream
        {
            Key = "es_video",
            VideoStream = videoStream
        };

        AudioStream audioStream = new AudioStream
        {
            Codec = "aac",
            ChannelCount = 2,
            BitrateBps = 160000
        };

        ElementaryStream elementaryStreamAudio = new ElementaryStream
        {
            Key = "es_audio",
            AudioStream = audioStream
        };

        MuxStream muxVideo = new MuxStream
        {
            Key = "mux_video",
            ElementaryStreams = { "es_video" },
            SegmentSettings = new SegmentSettings
            {
                SegmentDuration = new Google.Protobuf.WellKnownTypes.Duration
                {
                    Seconds = 2
                }
            }
        };

        MuxStream muxAudio = new MuxStream
        {
            Key = "mux_audio",
            ElementaryStreams = { "es_audio" },
            SegmentSettings = new SegmentSettings
            {
                SegmentDuration = new Google.Protobuf.WellKnownTypes.Duration
                {
                    Seconds = 2
                }
            }
        };

        CreateChannelRequest request = new CreateChannelRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, locationId),
            ChannelId = channelId,
            Channel = new Channel
            {
                InputAttachments = { inputAttachment },
                Output = new Channel.Types.Output
                {
                    Uri = outputUri
                },
                ElementaryStreams = { elementaryStreamVideo, elementaryStreamAudio },
                MuxStreams = { muxVideo, muxAudio },
                Manifests = {
                    new Manifest {
                        FileName = "manifest.m3u8",
                        Type = Manifest.Types.ManifestType.Hls,
                        MuxStreams = { "mux_video", "mux_audio" },
                        MaxSegmentCount = 5
                    }
                }
            }
        };

        // Make the request.
        Operation<Channel, OperationMetadata> response = await client.CreateChannelAsync(request);

        // Poll until the returned long-running operation is complete.
        Operation<Channel, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result.
        return completedResponse.Result;
    }
}
// [END livestream_create_channel]