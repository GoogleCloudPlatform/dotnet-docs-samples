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

using Google.Cloud.Video.LiveStream.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

[CollectionDefinition(nameof(LiveStreamFixture))]
public class LiveStreamFixture : IDisposable, IAsyncLifetime, ICollectionFixture<LiveStreamFixture>
{
    public string ProjectId { get; }
    public string LocationId { get; } = "us-central1";
    public string InputIdPrefix { get; } = "test-input";
    public string ChannelIdPrefix { get; } = "test-channel";
    public string EventIdPrefix { get; } = "test-event";
    public string AssetIdPrefix { get; } = "test-asset";
    public string AssetUri { get; } = "gs://cloud-samples-data/media/ForBiggerEscapes.mp4";
  
    public string ChannelOutputUri { get; } = "gs://my-bucket/my-output-folder/";

    public System.Int32 UpdateTopPixels { get; } = 5;
    public System.Int32 UpdateBottomPixels { get; } = 5;
    public Channel.Types.StreamingState ChannelStartedState { get; } = Channel.Types.StreamingState.AwaitingInput;
    public Channel.Types.StreamingState ChannelStoppedState { get; } = Channel.Types.StreamingState.Stopped;

    public List<string> InputIds { get; } = new List<string>();
    public List<string> ChannelIds { get; } = new List<string>();
    public List<string> AssetIds { get; } = new List<string>();

    public Input TestInput { get; set; }
    public string TestInputId { get; set; }
    public Input TestUpdateInput { get; set; }
    public string TestUpdateInputId { get; set; }
    public Channel TestChannel { get; set; }
    public string TestChannelId { get; set; }
    public string TestPoolId { get; } = "default"; // only 1 pool supported per location

    public Input TestInputForCreateEvent { get; set; }
    public string TestInputForCreateEventId { get; set; }
    public Channel TestChannelForCreateEvent { get; set; }
    public string TestChannelForCreateEventId { get; set; }

    public Input TestInputForDeleteEvent { get; set; }
    public string TestInputForDeleteEventId { get; set; }
    public Channel TestChannelForDeleteEvent { get; set; }
    public string TestChannelForDeleteEventId { get; set; }

    public Input TestInputForGetListEvent { get; set; }
    public string TestInputForGetListEventId { get; set; }
    public Channel TestChannelForGetListEvent { get; set; }
    public string TestChannelForGetListEventId { get; set; }

    public string TestEventId { get; set; }

    public Asset TestAsset { get; set; }
    public string TestAssetId { get; set; }

    private readonly CreateInputSample _createInputSample = new CreateInputSample();
    private readonly ListInputsSample _listInputsSample = new ListInputsSample();
    private readonly DeleteInputSample _deleteInputSample = new DeleteInputSample();
    private readonly CreateChannelSample _createChannelSample = new CreateChannelSample();
    private readonly ListChannelsSample _listChannelsSample = new ListChannelsSample();
    private readonly DeleteChannelSample _deleteChannelSample = new DeleteChannelSample();
    private readonly StartChannelSample _startChannelSample = new StartChannelSample();
    private readonly StopChannelSample _stopChannelSample = new StopChannelSample();
    private readonly ListChannelEventsSample _listChannelEventsSample = new ListChannelEventsSample();
    private readonly CreateChannelEventSample _createChannelEventSample = new CreateChannelEventSample();
    private readonly DeleteChannelEventSample _deleteChannelEventSample = new DeleteChannelEventSample();
    private readonly CreateAssetSample _createAssetSample = new CreateAssetSample();
    private readonly ListAssetsSample _listAssetsSample = new ListAssetsSample();
    private readonly DeleteAssetSample _deleteAssetSample = new DeleteAssetSample();

    public LiveStreamFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }
    }

    public async Task InitializeAsync()
    {
        await CleanOutdatedResources();

        TestInputId = $"{InputIdPrefix}-{RandomId()}";
        InputIds.Add(TestInputId);
        TestInput = await _createInputSample.CreateInputAsync(ProjectId, LocationId, TestInputId);

        TestUpdateInputId = $"{InputIdPrefix}-updated-{RandomId()}";
        InputIds.Add(TestUpdateInputId);
        TestUpdateInput = await _createInputSample.CreateInputAsync(ProjectId, LocationId, TestUpdateInputId);

        TestChannelId = $"{ChannelIdPrefix}-{RandomId()}";
        ChannelIds.Add(TestChannelId);
        TestChannel = await _createChannelSample.CreateChannelAsync(ProjectId, LocationId, TestChannelId, TestInputId, ChannelOutputUri);

        // Channel events
        TestEventId = $"{EventIdPrefix}-{RandomId()}";

        // Channel events - Create
        TestInputForCreateEventId = $"{InputIdPrefix}-{RandomId()}";
        InputIds.Add(TestInputForCreateEventId);
        TestInputForCreateEvent = await _createInputSample.CreateInputAsync(ProjectId, LocationId, TestInputForCreateEventId);

        TestChannelForCreateEventId = $"{ChannelIdPrefix}-{RandomId()}";
        ChannelIds.Add(TestChannelForCreateEventId);
        TestChannelForCreateEvent = await _createChannelSample.CreateChannelAsync(ProjectId, LocationId, TestChannelForCreateEventId, TestInputForCreateEventId, ChannelOutputUri);
        await _startChannelSample.StartChannelAsync(ProjectId, LocationId, TestChannelForCreateEventId);

        // Channel events - Get and List
        TestInputForGetListEventId = $"{InputIdPrefix}-{RandomId()}";
        InputIds.Add(TestInputForGetListEventId);
        TestInputForGetListEvent = await _createInputSample.CreateInputAsync(ProjectId, LocationId, TestInputForGetListEventId);

        TestChannelForGetListEventId = $"{ChannelIdPrefix}-{RandomId()}";
        ChannelIds.Add(TestChannelForGetListEventId);
        TestChannelForGetListEvent = await _createChannelSample.CreateChannelAsync(ProjectId, LocationId, TestChannelForGetListEventId, TestInputForGetListEventId, ChannelOutputUri);
        await _startChannelSample.StartChannelAsync(ProjectId, LocationId, TestChannelForGetListEventId);
        _createChannelEventSample.CreateChannelEvent(ProjectId, LocationId, TestChannelForGetListEventId, TestEventId);

        // Channel events - Delete
        TestInputForDeleteEventId = $"{InputIdPrefix}-{RandomId()}";
        InputIds.Add(TestInputForDeleteEventId);
        TestInputForDeleteEvent = await _createInputSample.CreateInputAsync(ProjectId, LocationId, TestInputForDeleteEventId);

        TestChannelForDeleteEventId = $"{ChannelIdPrefix}-{RandomId()}";
        ChannelIds.Add(TestChannelForDeleteEventId);
        TestChannelForDeleteEvent = await _createChannelSample.CreateChannelAsync(ProjectId, LocationId, TestChannelForDeleteEventId, TestInputForDeleteEventId, ChannelOutputUri);
        await _startChannelSample.StartChannelAsync(ProjectId, LocationId, TestChannelForDeleteEventId);
        _createChannelEventSample.CreateChannelEvent(ProjectId, LocationId, TestChannelForDeleteEventId, TestEventId);

        TestAssetId = $"{AssetIdPrefix}-{RandomId()}";
        AssetIds.Add(TestAssetId);
        TestAsset = await _createAssetSample.CreateAssetAsync(ProjectId, LocationId, TestAssetId, AssetUri);
    }

    public async Task CleanOutdatedResources()
    {
        int TWO_HOURS_IN_SECS = 7200;
        var channels = _listChannelsSample.ListChannels(ProjectId, LocationId);
        // Delete channels first
        foreach (Channel channel in channels)
        {
            string id = channel.ChannelName.ChannelId;
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long creation = channel.CreateTime.Seconds;
            if ((now - creation) >= TWO_HOURS_IN_SECS)
            {
                await DeleteChannel(id);
            }
        }
        // Delete inputs next. Delete input fails if it is attached to a channel.
        var inputs = _listInputsSample.ListInputs(ProjectId, LocationId);
        foreach (Input input in inputs)
        {
            string id = input.InputName.InputId;
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long creation = input.CreateTime.Seconds;
            if ((now - creation) >= TWO_HOURS_IN_SECS)
            {
                await DeleteInput(id);
            }
        }

        // Delete assets next.
        var assets = _listAssetsSample.ListAssets(ProjectId, LocationId);
        foreach (Asset asset in assets)
        {
            string id = asset.AssetName.AssetId;
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long creation = asset.CreateTime.Seconds;
            if ((now - creation) >= TWO_HOURS_IN_SECS)
            {
                await DeleteAsset(id);
            }
        }
    }

    public async Task DisposeAsync()
    {
        // Delete channels first
        foreach (string id in ChannelIds)
        {
            await DeleteChannel(id);
        }

        // Delete inputs next. Delete input fails if it is attached to a channel.
        foreach (string id in InputIds)
        {
            await DeleteInput(id);
        }

        // Delete assets next.
        foreach (string id in AssetIds)
        {
            await DeleteAsset(id);
        }
    }

    public async Task DeleteChannel(string id)
    {
        try
        {
            // Channel must be stopped before it can be deleted. Channel stop fails if the
            // channel is already in the stopped state.
            await _stopChannelSample.StopChannelAsync(ProjectId, LocationId, id);
        }
        catch (Exception e)
        {
            Console.WriteLine("Stop failed for channel: " + id + " with error: " + e.ToString());
        }

        try
        {
            // Channel events must be deleted before the channel can be deleted.
            var channelEvents = _listChannelEventsSample.ListChannelEvents(ProjectId, LocationId, id);
            foreach (Event channelEvent in channelEvents)
            {
                try
                {
                    _deleteChannelEventSample.DeleteChannelEvent(ProjectId, LocationId, id, channelEvent.EventName.EventId);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Delete failed for channel event: " + channelEvent.EventName.EventId + " with error: " + e.ToString());
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("List events failed for channel: " + id + " with error: " + e.ToString());
        }

        try
        {
            await _deleteChannelSample.DeleteChannelAsync(ProjectId, LocationId, id);
        }
        catch (Exception e)
        {
            Console.WriteLine("Delete failed for channel: " + id + " with error: " + e.ToString());
        }
    }

    public async Task DeleteInput(string id)
    {
        try
        {
            await _deleteInputSample.DeleteInputAsync(ProjectId, LocationId, id);
        }
        catch (Exception e)
        {
            Console.WriteLine("Delete failed for input: " + id + " with error: " + e.ToString());
        }
    }

    public async Task DeleteAsset(string id)
    {
        try
        {
            await _deleteAssetSample.DeleteAssetAsync(ProjectId, LocationId, id);
        }
        catch (Exception e)
        {
            Console.WriteLine("Delete failed for asset: " + id + " with error: " + e.ToString());
        }
    }

    public void Dispose()
    {
    }

    public string RandomId()
    {
        return $"cs-{System.Guid.NewGuid()}";
    }
}