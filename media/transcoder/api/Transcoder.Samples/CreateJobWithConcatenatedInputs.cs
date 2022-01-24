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

// [START transcoder_create_job_with_concatenated_inputs]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Transcoder.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class CreateJobWithConcatenatedInputsSample
{
    public Job CreateJobWithConcatenatedInputs(
        string projectId, string location, string inputUri1, TimeSpan startTimeInput1, TimeSpan endTimeInput1, string inputUri2, TimeSpan startTimeInput2, TimeSpan endTimeInput2, string outputUri)
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

        Input input1 = new Input
        {
            Key = "input1",
            Uri = inputUri1
        };

        Input input2 = new Input
        {
            Key = "input2",
            Uri = inputUri2
        };

        EditAtom atom1 = new EditAtom
        {
            Key = "atom1",
            StartTimeOffset = Duration.FromTimeSpan(startTimeInput1),
            EndTimeOffset = Duration.FromTimeSpan(endTimeInput1),
            Inputs = { input1.Key }
        };

        EditAtom atom2 = new EditAtom
        {
            Key = "atom2",
            StartTimeOffset = Duration.FromTimeSpan(startTimeInput2),
            EndTimeOffset = Duration.FromTimeSpan(endTimeInput2),
            Inputs = { input2.Key }
        };

        Output output = new Output
        {
            Uri = outputUri
        };

        JobConfig jobConfig = new JobConfig
        {
            Inputs = { input1, input2 },
            EditList = { atom1, atom2 },
            Output = output,
            ElementaryStreams = { elementaryStream0, elementaryStream1 },
            MuxStreams = { muxStream0 }
        };

        // Build the job.
        Job newJob = new Job
        {
            OutputUri = outputUri,
            Config = jobConfig
        };

        // Call the API.
        Job job = client.CreateJob(parent, newJob);

        // Return the result.
        return job;
    }
}
// [END transcoder_create_job_with_concatenated_inputs]