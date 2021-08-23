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
    public string CreateJobWithSetNumberImagesSpritesheet(
        string projectId, string location, string inputUri, string outputUri)
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

        // Generates a 10x10 spritesheet of small images from the input video. To preserve the source
        // aspect ratio, you should set the spriteWidthPixels field or the spriteHeightPixels
        // field, but not both.
        SpriteSheet smallSpriteSheet = new SpriteSheet();
        smallSpriteSheet.FilePrefix = smallSpritesheetFilePrefix;
        smallSpriteSheet.SpriteHeightPixels = 32;
        smallSpriteSheet.SpriteWidthPixels = 64;
        smallSpriteSheet.ColumnCount = 10;
        smallSpriteSheet.RowCount = 10;
        smallSpriteSheet.TotalCount = 100;

        // Generates a spritesheet of larger images taken periodically from the input video.
        SpriteSheet largeSpriteSheet = new SpriteSheet();
        largeSpriteSheet.FilePrefix = largeSpritesheetFilePrefix;
        largeSpriteSheet.SpriteHeightPixels = 72;
        largeSpriteSheet.SpriteWidthPixels = 128;
        largeSpriteSheet.ColumnCount = 10;
        largeSpriteSheet.RowCount = 10;
        largeSpriteSheet.TotalCount = 100;

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
        jobConfig.SpriteSheets.Add(smallSpriteSheet);
        jobConfig.SpriteSheets.Add(largeSpriteSheet);

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
// [END transcoder_create_job_with_set_number_images_spritesheet]