// Copyright(c) 2017 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START video_quickstart]

using Google.Cloud.VideoIntelligence.V1;
using System;

namespace GoogleCloudSamples.VideoIntelligence
{
    public class QuickStart
    {
        public static void Main(string[] args)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputUri = @"gs://demomaker/cat.mp4",
                Features = { Feature.LabelDetection }
            };
            var op = client.AnnotateVideo(request).PollUntilCompleted();
            foreach (var result in op.Result.AnnotationResults)
            {
                foreach (var annotation in result.SegmentLabelAnnotations)
                {
                    Console.WriteLine($"Video label: {annotation.Entity.Description}");
                    foreach (var entity in annotation.CategoryEntities)
                    {
                        Console.WriteLine($"Video label category: {entity.Description}");
                    }
                    foreach (var segment in annotation.Segments)
                    {
                        Console.Write("Segment location: ");
                        Console.Write(segment.Segment.StartTimeOffset);
                        Console.Write(":");
                        Console.WriteLine(segment.Segment.EndTimeOffset);
                        System.Console.WriteLine($"Confidence: {segment.Confidence}");
                    }
                }
            }
        }
    }
}
// [END video_quickstart]
