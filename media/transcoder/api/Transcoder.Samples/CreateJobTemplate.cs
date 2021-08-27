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
    public JobTemplate CreateJobTemplate(
        string projectId, string location, string templateId)
    {
        // Create the client.
        TranscoderServiceClient client = TranscoderServiceClient.Create();

        // Build the parent location name.
        LocationName parentLocation = new LocationName(projectId, location);

        // Build the job template config.
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

        VideoStream videoStream1 = new VideoStream
        {
            H264 = new VideoStream.Types.H264CodecSettings
            {
                BitrateBps = 2500000,
                FrameRate = 60,
                HeightPixels = 720,
                WidthPixels = 1280
            }
        };

        AudioStream audioStream0 = new AudioStream
        {
            Codec = "aac",
            BitrateBps = 64000
        };

        ElementaryStream elementaryStream0 = new ElementaryStream
        {
            Key = "video_stream0",
            VideoStream = videoStream0
        };

        ElementaryStream elementaryStream1 = new ElementaryStream
        {
            Key = "video_stream1",
            VideoStream = videoStream1
        };

        ElementaryStream elementaryStream2 = new ElementaryStream
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

        MuxStream muxStream1 = new MuxStream
        {
            Key = "hd",
            Container = "mp4",
            ElementaryStreams = { "video_stream1", "audio_stream0" }
        };

        JobConfig jobConfig = new JobConfig
        {
            ElementaryStreams = { elementaryStream0, elementaryStream1, elementaryStream2 },
            MuxStreams = { muxStream0, muxStream1 }
        };

        JobTemplate newJobTemplate = new JobTemplate
        {
            Config = jobConfig
        };

        // Call the API.
        JobTemplate jobTemplate = client.CreateJobTemplate(parentLocation, newJobTemplate, templateId);
        return jobTemplate;
    }
}
// [END transcoder_create_job_template]
