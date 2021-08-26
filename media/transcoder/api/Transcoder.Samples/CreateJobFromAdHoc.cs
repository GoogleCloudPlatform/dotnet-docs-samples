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
    public string CreateJobFromAdHoc(
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
                WidthPixels = 640,
            }
        };

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

        MuxStream muxStream0 = new MuxStream
        {
            Key = "sd",
            Container = "mp4",
            ElementaryStreams = {"video_stream0", "audio_stream0"}
        };

        MuxStream muxStream1 = new MuxStream();
        muxStream1.Key = "hd";
        muxStream1.Container = "mp4";
        muxStream1.ElementaryStreams.Add("video_stream1");
        muxStream1.ElementaryStreams.Add("audio_stream0");

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
        jobConfig.ElementaryStreams.Add(elementaryStream2);
        jobConfig.MuxStreams.Add(muxStream0);
        jobConfig.MuxStreams.Add(muxStream1);

        // Build the job.
        Job job = new Job();
        job.Config = jobConfig;
        job.InputUri = inputUri;
        job.OutputUri = outputUri;

        // Call the API.
        Job response = client.CreateJob(parent, job);

        // Return the result.
        return "Job: " + response.JobName;
    }
}
// [END transcoder_create_job_from_ad_hoc]