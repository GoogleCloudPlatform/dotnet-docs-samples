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

// [START transcoder_create_job_with_static_overlay]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Transcoder.V1;

public class CreateJobWithStaticOverlaySample
{
    public string CreateJobWithStaticOverlay(
        string projectId, string location, string inputUri, string overlayImageUri, string outputUri)
    {
        // Create the client.
        TranscoderServiceClient client = TranscoderServiceClient.Create();

        // Build the parent location name.
        LocationName parent = new LocationName(projectId, location);

        // Build the job config.
        VideoStream videoStream0 = new VideoStream();
        videoStream0.H264 = new VideoStream.Types.H264CodecSettings();
        videoStream0.H264.BitrateBps = 550000;
        videoStream0.H264.FrameRate = 60;
        videoStream0.H264.HeightPixels = 360;
        videoStream0.H264.WidthPixels = 640;

        AudioStream audioStream0 = new AudioStream();
        audioStream0.Codec = "aac";
        audioStream0.BitrateBps = 64000;

        // Create the overlay image. Only JPEG is supported. Image resolution is based on output
        // video resolution. To respect the original image aspect ratio, set either x or y to 0.0.
        // This example stretches the overlay image the full width and half of the height of the
        // output video.
        Overlay.Types.NormalizedCoordinate normalizedCoordinate = new Overlay.Types.NormalizedCoordinate();
        normalizedCoordinate.X = 1;
        normalizedCoordinate.Y = 0.5;
        Overlay.Types.Image overlayImage = new Overlay.Types.Image();
        overlayImage.Uri = overlayImageUri;
        overlayImage.Alpha = 1;
        overlayImage.Resolution = normalizedCoordinate;

        // Create the starting animation (when the overlay appears). Use the values x: 0 and y: 0 to
        // position the top-left corner of the overlay in the top-left corner of the output video.
        Overlay.Types.Animation animationStart = new Overlay.Types.Animation();
        animationStart.AnimationStatic = new Overlay.Types.AnimationStatic();
        animationStart.AnimationStatic.Xy = new Overlay.Types.NormalizedCoordinate();
        animationStart.AnimationStatic.Xy.X = 0;
        animationStart.AnimationStatic.Xy.Y = 0;
        animationStart.AnimationStatic.StartTimeOffset = new Google.Protobuf.WellKnownTypes.Duration();
        animationStart.AnimationStatic.StartTimeOffset.Seconds = 0;

        // Create the ending animation (when the overlay disappears). In this example, the overlay
        // disappears at the 10-second mark in the output video.
        Overlay.Types.Animation animationEnd = new Overlay.Types.Animation();
        animationEnd.AnimationEnd = new Overlay.Types.AnimationEnd();
        animationEnd.AnimationEnd.StartTimeOffset = new Google.Protobuf.WellKnownTypes.Duration();
        animationEnd.AnimationEnd.StartTimeOffset.Seconds = 10;

        // Create the overlay and add the image and animations to it.
        Overlay overlay = new Overlay();
        overlay.Image = overlayImage;
        overlay.Animations.Add(animationStart);
        overlay.Animations.Add(animationEnd);

        ElementaryStream elementaryStream0 = new ElementaryStream();
        elementaryStream0.Key = "video_stream0";
        elementaryStream0.VideoStream = videoStream0;

        ElementaryStream elementaryStream1 = new ElementaryStream();
        elementaryStream1.Key = "audio_stream0";
        elementaryStream1.AudioStream = audioStream0;

        MuxStream muxStream0 = new MuxStream();
        muxStream0.Key = "sd";
        muxStream0.Container = "mp4";
        muxStream0.ElementaryStreams.Add("video_stream0");
        muxStream0.ElementaryStreams.Add("audio_stream0");

        Input input = new Input();
        input.Key = "input0";
        input.Uri = inputUri;

        Output output = new Output();
        output.Uri = outputUri;

        JobConfig jobConfig = new JobConfig();
        jobConfig.Inputs.Add(input);
        jobConfig.Output = output;
        jobConfig.ElementaryStreams.Add(elementaryStream0);
        jobConfig.ElementaryStreams.Add(elementaryStream1);
        jobConfig.MuxStreams.Add(muxStream0);
        jobConfig.Overlays.Add(overlay);

        // Build the job.
        Job job = new Job();
        job.InputUri = inputUri;
        job.OutputUri = outputUri;
        job.Config = jobConfig;

        // Call the API.
        Job response = client.CreateJob(parent, job);

        // Return the result.
        return "Job: " + response.JobName;
    }
}
// [END transcoder_create_job_with_static_overlay]