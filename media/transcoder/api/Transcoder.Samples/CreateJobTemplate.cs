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

// [START transcoder_create_job_template]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Transcoder.V1;

public class CreateJobTemplateSample
{
    public string CreateJobTemplate(
        string projectId, string location, string templateId)
    {
        // Create the client.
        TranscoderServiceClient client = TranscoderServiceClient.Create();

        // Build the parent location name.
        LocationName parentLocation = new LocationName(projectId, location);

        // Build the job template config.
        VideoStream videoStream0 = new VideoStream();
        videoStream0.H264 = new VideoStream.Types.H264CodecSettings();
        videoStream0.H264.BitrateBps = 550000;
        videoStream0.H264.FrameRate = 60;
        videoStream0.H264.HeightPixels = 360;
        videoStream0.H264.WidthPixels = 640;

        VideoStream videoStream1 = new VideoStream();
        videoStream1.H264 = new VideoStream.Types.H264CodecSettings();
        videoStream1.H264.BitrateBps = 2500000;
        videoStream1.H264.FrameRate = 60;
        videoStream1.H264.HeightPixels = 720;
        videoStream1.H264.WidthPixels = 1280;

        AudioStream audioStream0 = new AudioStream();
        audioStream0.Codec = "aac";
        audioStream0.BitrateBps = 64000;

        ElementaryStream elementaryStream0 = new ElementaryStream();
        elementaryStream0.Key = "video_stream0";
        elementaryStream0.VideoStream = videoStream0;

        ElementaryStream elementaryStream1 = new ElementaryStream();
        elementaryStream1.Key = "video_stream1";
        elementaryStream1.VideoStream = videoStream1;

        ElementaryStream elementaryStream2 = new ElementaryStream();
        elementaryStream2.Key = "audio_stream0";
        elementaryStream2.AudioStream = audioStream0;

        MuxStream muxStream0 = new MuxStream();
        muxStream0.Key = "sd";
        muxStream0.Container = "mp4";
        muxStream0.ElementaryStreams.Add("video_stream0");
        muxStream0.ElementaryStreams.Add("audio_stream0");

        MuxStream muxStream1 = new MuxStream();
        muxStream1.Key = "hd";
        muxStream1.Container = "mp4";
        muxStream1.ElementaryStreams.Add("video_stream1");
        muxStream1.ElementaryStreams.Add("audio_stream0");

        JobConfig jobConfig = new JobConfig();
        jobConfig.ElementaryStreams.Add(elementaryStream0);
        jobConfig.ElementaryStreams.Add(elementaryStream1);
        jobConfig.ElementaryStreams.Add(elementaryStream2);
        jobConfig.MuxStreams.Add(muxStream0);
        jobConfig.MuxStreams.Add(muxStream1);

        JobTemplate jobTemplate = new JobTemplate();
        jobTemplate.Config = jobConfig;

        // Call the API.
        JobTemplate response = client.CreateJobTemplate(parentLocation, jobTemplate, templateId);
        return "Job template: " + response.Name;
    }
}
// [END transcoder_create_job_template]
