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

// [START transcoder_create_job_with_animated_overlay]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Transcoder.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class CreateJobWithAnimatedOverlaySample
{
    public Job CreateJobWithAnimatedOverlay(
        string projectId, string location, string inputUri, string overlayImageUri, string outputUri)
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

        // Create the overlay image. Image resolution is based on output video resolution.
        // This example uses the values x: 0 and y: 0 to maintain the original resolution
        // of the overlay image.
        Overlay.Types.Image overlayImage = new Overlay.Types.Image
        {
            Uri = overlayImageUri,
            Alpha = 1,
            Resolution = new Overlay.Types.NormalizedCoordinate
            {
                X = 0,
                Y = 0
            }
        };

        // Create the starting animation (when the overlay starts to fade in). Use the values x: 0.5
        // and y: 0.5 to position the top-left corner of the overlay in the center of the output
        // video.
        Overlay.Types.Animation animationFadeIn = new Overlay.Types.Animation
        {
            AnimationFade = new Overlay.Types.AnimationFade
            {
                FadeType = Overlay.Types.FadeType.FadeIn,
                Xy = new Overlay.Types.NormalizedCoordinate
                {
                    X = 0.5,
                    Y = 0.5
                },
                StartTimeOffset = Duration.FromTimeSpan(TimeSpan.FromSeconds(5)),
                EndTimeOffset = Duration.FromTimeSpan(TimeSpan.FromSeconds(10))
            }
        };

        // Create the ending animation (when the overlay starts to fade out). The overlay will start
        // to fade out at the 12-second mark in the output video.
        Overlay.Types.Animation animationFadeOut = new Overlay.Types.Animation
        {
            AnimationFade = new Overlay.Types.AnimationFade
            {
                FadeType = Overlay.Types.FadeType.FadeOut,
                Xy = new Overlay.Types.NormalizedCoordinate
                {
                    X = 0.5,
                    Y = 0.5
                },
                StartTimeOffset = Duration.FromTimeSpan(TimeSpan.FromSeconds(12)),
                EndTimeOffset = Duration.FromTimeSpan(TimeSpan.FromSeconds(15))
            }
        };

        // Create the overlay and add the image and animations to it.
        Overlay overlay = new Overlay
        {
            Image = overlayImage,
            Animations = { animationFadeIn, animationFadeOut }
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
            Overlays = { overlay }
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
// [END transcoder_create_job_with_animated_overlay]