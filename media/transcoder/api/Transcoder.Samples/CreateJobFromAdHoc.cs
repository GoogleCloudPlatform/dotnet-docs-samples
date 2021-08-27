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

// [START transcoder_create_job_from_ad_hoc]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Transcoder.V1;

public class CreateJobFromAdHocSample
{
    public Job CreateJobFromAdHoc(
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
            ElementaryStreams = { elementaryStream0, elementaryStream1, elementaryStream2 },
            MuxStreams = { muxStream0, muxStream1 }
        };

        // Build the job.
        Job newJob = new Job
        {
            Config = jobConfig,
            InputUri = inputUri,
            OutputUri = outputUri
        };

        // Call the API.
        Job job = client.CreateJob(parent, newJob);

        // Return the result.
        return job;
    }
}
// [END transcoder_create_job_from_ad_hoc]