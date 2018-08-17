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

using CommandLine;
using Google.Cloud.VideoIntelligence.V1;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace GoogleCloudSamples.VideoIntelligence
{
    [Verb("labels", HelpText = "Print a list of labels found in the video.")]
    class AnalyzeLabelsOptions
    {
        [Value(0, HelpText = "The uri of the video to examine. "
            + "Can be path to a local file or a Cloud storage uri like "
            + "gs://bucket/object.",
            Required = true)]
        public string Uri { get; set; }
    }

    [Verb("shots", HelpText = "Print a list shot changes.")]
    class AnalyzeShotsOptions
    {
        [Value(0, HelpText = "The uri of the video to examine. "
            + "Must be a Cloud storage uri like "
            + "gs://bucket/object.",
            Required = true)]
        public string Uri { get; set; }
    }

    [Verb("explicit-content", HelpText = "Analyze the content of the video.")]
    class AnalyzeExplicitContentOptions
    {
        [Value(0, HelpText = "The uri of the video to examine. "
            + "Must be a Cloud storage uri like "
            + "gs://bucket/object.",
            Required = true)]
        public string Uri { get; set; }
    }

    public class Analyzer
    {
        // [START video_analyze_shots]
        public static object AnalyzeShotsGcs(string uri)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputUri = uri,
                Features = { Feature.ShotChangeDetection }
            };
            var op = client.AnnotateVideo(request).PollUntilCompleted();
            foreach (var result in op.Result.AnnotationResults)
            {
                foreach (var annotation in result.ShotAnnotations)
                {
                    Console.Out.WriteLine("Start Time Offset: {0}\tEnd Time Offset: {1}",
                        annotation.StartTimeOffset, annotation.EndTimeOffset);
                }
            }
            return 0;
        }
        // [END video_analyze_shots]

        // [START video_analyze_labels]
        public static object AnalyzeLabels(string path)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputContent = Google.Protobuf.ByteString.CopyFrom(File.ReadAllBytes(path)),
                Features = { Feature.LabelDetection }
            };
            var op = client.AnnotateVideo(request).PollUntilCompleted();
            foreach (var result in op.Result.AnnotationResults)
            {
                PrintLabels("Video", result.SegmentLabelAnnotations);
                PrintLabels("Shot", result.ShotLabelAnnotations);
                PrintLabels("Frame", result.FrameLabelAnnotations);
            }
            return 0;
        }

        // [END video_analyze_labels]

        // [START video_analyze_labels_gcs]
        public static object AnalyzeLabelsGcs(string uri)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputUri = uri,
                Features = { Feature.LabelDetection }
            };
            var op = client.AnnotateVideo(request).PollUntilCompleted();
            foreach (var result in op.Result.AnnotationResults)
            {
                PrintLabels("Video", result.SegmentLabelAnnotations);
                PrintLabels("Shot", result.ShotLabelAnnotations);
                PrintLabels("Frame", result.FrameLabelAnnotations);
            }
            return 0;
        }

        // [START video_analyze_labels]
        static void PrintLabels(string labelName,
            IEnumerable<LabelAnnotation> labelAnnotations)
        {
            foreach (var annotation in labelAnnotations)
            {
                Console.WriteLine($"{labelName} label: {annotation.Entity.Description}");
                foreach (var entity in annotation.CategoryEntities)
                {
                    Console.WriteLine($"{labelName} label category: {entity.Description}");
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
        // [END video_analyze_labels]
        // [END video_analyze_labels_gcs]

        // [START video_analyze_explicit_content]
        public static object AnalyzeExplicitContentGcs(string uri)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputUri = uri,
                Features = { Feature.ExplicitContentDetection }
            };
            var op = client.AnnotateVideo(request).PollUntilCompleted();
            foreach (var result in op.Result.AnnotationResults)
            {
                foreach (var frame in result.ExplicitAnnotation.Frames)
                {
                    Console.WriteLine("Time Offset: {0}", frame.TimeOffset);
                    Console.WriteLine("Pornography Likelihood: {0}", frame.PornographyLikelihood);
                    Console.WriteLine();
                }
            }
            return 0;
        }
        // [END video_analyze_explicit_content]

        public static void Main(string[] args)
        {
            var verbMap = new VerbMap<object>()
                .Add((AnalyzeShotsOptions opts) => AnalyzeShotsGcs(opts.Uri))
                .Add((AnalyzeExplicitContentOptions opts) => AnalyzeExplicitContentGcs(opts.Uri))
                .Add((AnalyzeLabelsOptions opts) => IsStorageUri(opts.Uri) ? AnalyzeLabelsGcs(opts.Uri) : AnalyzeLabels(opts.Uri))
                .SetNotParsedFunc((errs) => 1);
            verbMap.Run(args);
        }
        static bool IsStorageUri(string s) => s.Substring(0, 4).ToLower() == "gs:/";
    }
}
