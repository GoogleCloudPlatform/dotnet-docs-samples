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

// [START livestream_create_channel_event]

using Google.Cloud.Video.LiveStream.V1;

public class CreateChannelEventSample
{
    public Event CreateChannelEvent(
         string projectId, string locationId, string channelId, string eventId)
    {
        // Create the client.
        LivestreamServiceClient client = LivestreamServiceClient.Create();

        CreateEventRequest request = new CreateEventRequest
        {
            ParentAsChannelName = ChannelName.FromProjectLocationChannel(projectId, locationId, channelId),
            EventId = eventId,
            Event = new Event
            {
                AdBreak = new Event.Types.AdBreakTask
                {
                    Duration = new Google.Protobuf.WellKnownTypes.Duration
                    {
                        Seconds = 30
                    }
                },
                ExecuteNow = true
            }
        };

        // Make the request.
        Event response = client.CreateEvent(request);
        return response;
    }
}
// [END livestream_create_channel_event]