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

// [START transcoder_create_job_with_set_number_images_spritesheet]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Transcoder.V1;

public class CreateJobWithSetNumberImagesSpritesheetSample
{
    public static string smallSpritesheetFilePrefix = "small-sprite-sheet";
    public static string largeSpritesheetFilePrefix = "large-sprite-sheet";
    public static string spritesheetFileSuffix = "0000000000.jpeg";
    public Job CreateJobWithSetNumberImagesSpritesheet(
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

        // Generates a 10x10 spritesheet of small images from the input video. To preserve the source
        // aspect ratio, you should set the spriteWidthPixels field or the spriteHeightPixels
        // field, but not both.
        SpriteSheet smallSpriteSheet = new SpriteSheet
        {
            FilePrefix = smallSpritesheetFilePrefix,
            SpriteHeightPixels = 32,
            SpriteWidthPixels = 64,
            ColumnCount = 10,
            RowCount = 10,
            TotalCount = 100
        };

        // Generates a spritesheet of larger images taken periodically from the input video.
        SpriteSheet largeSpriteSheet = new SpriteSheet
        {
            FilePrefix = largeSpritesheetFilePrefix,
            SpriteHeightPixels = 72,
            SpriteWidthPixels = 128,
            ColumnCount = 10,
            RowCount = 10,
            TotalCount = 100
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
        Job newJob = new Job();
        newJob.InputUri = inputUri;
        newJob.OutputUri = outputUri;
        newJob.Config = jobConfig;

        // Call the API.
        Job job = client.CreateJob(parent, newJob);

        // Return the result.
        return job;
    }
}
// [END transcoder_create_job_with_set_number_images_spritesheet]