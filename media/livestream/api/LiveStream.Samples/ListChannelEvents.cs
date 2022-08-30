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

// [START livestream_list_channel_events]

using Google.Api.Gax;
using Google.Cloud.Video.LiveStream.V1;
using System.Collections.Generic;
using System.Linq;

public class ListChannelEventsSample
{
    public IList<Event> ListChannelEvents(
        string projectId, string regionId, string channelId)
    {
        // Create the client.
        LivestreamServiceClient client = LivestreamServiceClient.Create();

        ListEventsRequest request = new ListEventsRequest
        {
            ParentAsChannelName = ChannelName.FromProjectLocationChannel(projectId, regionId, channelId)
        };

        // Make the request.
        PagedEnumerable<ListEventsResponse, Event> response = client.ListEvents(request);

        // The returned sequence will lazily perform RPCs as it's being iterated over.
        return response.ToList();
    }
}
// [END livestream_list_channel_events]