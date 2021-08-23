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

public class CreateJobWithAnimatedOverlaySample
{
    public string CreateJobWithAnimatedOverlay(
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
        // video resolution. This example uses the values x: 0 and y: 0 to maintain the original
        // resolution of the overlay image.
        Overlay.Types.NormalizedCoordinate normalizedCoordinate = new Overlay.Types.NormalizedCoordinate();
        normalizedCoordinate.X = 0;
        normalizedCoordinate.Y = 0;
        Overlay.Types.Image overlayImage = new Overlay.Types.Image();
        overlayImage.Uri = overlayImageUri;
        overlayImage.Alpha = 1;
        overlayImage.Resolution = normalizedCoordinate;

        // Create the starting animation (when the overlay starts to fade in). Use the values x: 0.5
        // and y: 0.5 to position the top-left corner of the overlay in the top-left corner of the
        // output video.
        Overlay.Types.Animation animationFadeIn = new Overlay.Types.Animation();
        animationFadeIn.AnimationFade = new Overlay.Types.AnimationFade();
        animationFadeIn.AnimationFade.FadeType = Overlay.Types.FadeType.FadeIn;
        animationFadeIn.AnimationFade.Xy = new Overlay.Types.NormalizedCoordinate();
        animationFadeIn.AnimationFade.Xy.X = 0.5;
        animationFadeIn.AnimationFade.Xy.Y = 0.5;
        animationFadeIn.AnimationFade.StartTimeOffset = new Google.Protobuf.WellKnownTypes.Duration();
        animationFadeIn.AnimationFade.StartTimeOffset.Seconds = 5;
        animationFadeIn.AnimationFade.EndTimeOffset = new Google.Protobuf.WellKnownTypes.Duration();
        animationFadeIn.AnimationFade.EndTimeOffset.Seconds = 10;

        // Create the ending animation (when the overlay starts to fade out). The overlay will start
        // to fade out at the 12-second mark in the output video.
        Overlay.Types.Animation animationFadeOut = new Overlay.Types.Animation();
        animationFadeOut.AnimationFade = new Overlay.Types.AnimationFade();
        animationFadeOut.AnimationFade.FadeType = Overlay.Types.FadeType.FadeOut;
        animationFadeOut.AnimationFade.Xy = new Overlay.Types.NormalizedCoordinate();
        animationFadeOut.AnimationFade.Xy.X = 0.5;
        animationFadeOut.AnimationFade.Xy.Y = 0.5;
        animationFadeOut.AnimationFade.StartTimeOffset = new Google.Protobuf.WellKnownTypes.Duration();
        animationFadeOut.AnimationFade.StartTimeOffset.Seconds = 12;
        animationFadeOut.AnimationFade.EndTimeOffset = new Google.Protobuf.WellKnownTypes.Duration();
        animationFadeOut.AnimationFade.EndTimeOffset.Seconds = 15;

        // Create the overlay and add the image and animations to it.
        Overlay overlay = new Overlay();
        overlay.Image = overlayImage;
        overlay.Animations.Add(animationFadeIn);
        overlay.Animations.Add(animationFadeOut);

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
// [END transcoder_create_job_with_animated_overlay]