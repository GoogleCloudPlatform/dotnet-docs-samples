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
    public string ChannelOutputUri { get; } = "gs://my-bucket/my-output-folder/";

    public System.Int32 UpdateTopPixels { get; } = 5;
    public System.Int32 UpdateBottomPixels { get; } = 5;
    public Channel.Types.StreamingState ChannelStartedState { get; } = Channel.Types.StreamingState.AwaitingInput;
    public Channel.Types.StreamingState ChannelStoppedState { get; } = Channel.Types.StreamingState.Stopped;


    public List<string> InputIds { get; } = new List<string>();
    public List<string> ChannelIds { get; } = new List<string>();

    public Input TestInput { get; set; }
    public string TestInputId { get; set; }
    public Input TestUpdateInput { get; set; }
    public string TestUpdateInputId { get; set; }
    public Channel TestChannel { get; set; }
    public string TestChannelId { get; set; }

    private readonly CreateInputSample _createInputSample = new CreateInputSample();
    private readonly DeleteInputSample _deleteInputSample = new DeleteInputSample();
    private readonly CreateChannelSample _createChannelSample = new CreateChannelSample();
    private readonly DeleteChannelSample _deleteChannelSample = new DeleteChannelSample();
    private readonly StopChannelSample _stopChannelSample = new StopChannelSample();

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
        TestInputId = $"{InputIdPrefix}-{RandomId()}";
        InputIds.Add(TestInputId);
        TestInput = await _createInputSample.CreateInputAsync(ProjectId, LocationId, TestInputId);

        TestUpdateInputId = $"{InputIdPrefix}-updated-{RandomId()}";
        InputIds.Add(TestUpdateInputId);
        TestUpdateInput = await _createInputSample.CreateInputAsync(ProjectId, LocationId, TestUpdateInputId);

        TestChannelId = $"{ChannelIdPrefix}-{RandomId()}";
        ChannelIds.Add(TestChannelId);
        TestChannel = await _createChannelSample.CreateChannelAsync(ProjectId, LocationId, TestChannelId, TestInputId, ChannelOutputUri);
    }

    public async Task DisposeAsync()
    {
        // Delete channels first
        foreach (string id in ChannelIds)
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
                await _deleteChannelSample.DeleteChannelAsync(ProjectId, LocationId, id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed for channel: " + id + " with error: " + e.ToString());
            }
        }

        // Delete inputs next. Delete input fails if it is attached to a channel.
        foreach (string id in InputIds)
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
    }

    public void Dispose()
    {
    }

    public string RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }
}