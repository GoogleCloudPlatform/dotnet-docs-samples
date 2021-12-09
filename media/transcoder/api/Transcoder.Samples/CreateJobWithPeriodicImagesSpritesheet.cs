/*
 * Copyright 2021 Google LLC
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

// [START transcoder_create_job_with_periodic_images_spritesheet]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Transcoder.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class CreateJobWithPeriodicImagesSpritesheetSample
{
    public const string SmallSpritesheetFilePrefix = "small-sprite-sheet";
    public const string LargeSpritesheetFilePrefix = "large-sprite-sheet";
    public Job CreateJobWithPeriodicImagesSpritesheet(
        string projectId, string location, string inputUri, string outputUri)
    {
        // Create the client.
        TranscoderServiceClient client = TranscoderServiceClient.Create();

        // Build the parent location name.
        LocationName parent = new LocationName(projectId, location);

        // Build the job config.
        VideoStream videoStream0 = new VideoStream
        {
            H264 = new VideoStream.Types.H264CodecSettings
            {
                BitrateBps = 550000,
                FrameRate = 60,
                HeightPixels = 360,
                WidthPixels = 640
            }
        };

        AudioStream audioStream0 = new AudioStream
        {
            Codec = "aac",
            BitrateBps = 64000
        };

        // Generates a spritesheet of small images taken periodically from the
        // input video. To preserve the source aspect ratio, you should set the
        // SpriteWidthPixels field or the SpriteHeightPixels field, but not
        // both (the API will automatically calculate the missing field). For
        // this sample, we don't care about the aspect ratio so we set both
        // fields.
        SpriteSheet smallSpriteSheet = new SpriteSheet
        {
            FilePrefix = SmallSpritesheetFilePrefix,
            SpriteHeightPixels = 32,
            SpriteWidthPixels = 64,
            Interval = Duration.FromTimeSpan(TimeSpan.FromSeconds(7))
        };

        // Generates a spritesheet of larger images taken periodically from the
        // input video. To preserve the source aspect ratio, you should set the
        // SpriteWidthPixels field or the SpriteHeightPixels field, but not
        // both (the API will automatically calculate the missing field). For
        // this sample, we don't care about the aspect ratio so we set both
        // fields.
        SpriteSheet largeSpriteSheet = new SpriteSheet
        {
            FilePrefix = LargeSpritesheetFilePrefix,
            SpriteHeightPixels = 72,
            SpriteWidthPixels = 128,
            Interval = Duration.FromTimeSpan(TimeSpan.FromSeconds(7))
        };

        ElementaryStream elementaryStream0 = new ElementaryStream
        {
            Key = "video_stream0",
            VideoStream = videoStream0
        };

        ElementaryStream elementaryStream1 = new ElementaryStream
        {
            Key = "audio_stream0",
            AudioStream = audioStream0
        };

        MuxStream muxStream0 = new MuxStream
        {
            Key = "sd",
            Container = "mp4",
            ElementaryStreams = { "video_stream0", "audio_stream0" }
        };

        Input input = new Input
        {
            Key = "input0",
            Uri = inputUri
        };

        Output output = new Output
        {
            Uri = outputUri
        };

        JobConfig jobConfig = new JobConfig
        {
            Inputs = { input },
            Output = output,
            ElementaryStreams = { elementaryStream0, elementaryStream1 },
            MuxStreams = { muxStream0 },
            SpriteSheets = { smallSpriteSheet, largeSpriteSheet }
        };

        // Build the job.
        Job newJob = new Job
        {
            InputUri = inputUri,
            OutputUri = outputUri,
            Config = jobConfig
        };

        // Call the API.
        Job job = client.CreateJob(parent, newJob);

        // Return the result.
        return job;
    }
}
// [END transcoder_create_job_with_periodic_images_spritesheet]