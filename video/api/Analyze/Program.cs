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
using Google.Cloud.VideoIntelligence.V1Beta1;
using System;
using System.IO;
using System.Linq;

namespace GoogleCloudSamples.VideoIntelligence
{
    class VideoOptions
    {
        [Value(0, HelpText = "The uri of the video to examine. "
            + "Can be path to a local file or a Cloud storage uri like "
            + "gs://bucket/object.",
            Required = true)]
        public string Uri { get; set; }
    }

    [Verb("labels", HelpText = "Print a list of labels found in the video.")]
    class AnalyzeLabelsOptions : VideoOptions { }

    [Verb("shots", HelpText = "Print a list shot changes.")]
    class AnalyzeShotsOptions : VideoOptions { }

    [Verb("faces", HelpText = "Print the offsets when faces appear.")]
    class AnalyzeFacesOptions : VideoOptions { }

    public class Analyzer
    {
        // [START analyze_shots]
        public static object AnalyzeShots(string path)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputContent = Convert.ToBase64String(File.ReadAllBytes(path)),
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
        // [END analyze_shots]

        // [START analyze_shots_gcs]
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
        // [END analyze_shots_gcs]

        // [START analyze_labels]
        public static object AnalyzeLabels(string path)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputContent = Convert.ToBase64String(File.ReadAllBytes(path)),
                Features = { Feature.LabelDetection }
            };
            var op = client.AnnotateVideo(request).PollUntilCompleted();
            foreach (var result in op.Result.AnnotationResults)
            {
                foreach (var annotation in result.LabelAnnotations)
                {
                    Console.WriteLine(annotation.Description);
                }
            }
            return 0;
        }
        // [END analyze_labels]

        // [START analyze_labels_gcs]
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
                foreach (var annotation in result.LabelAnnotations)
                {
                    Console.WriteLine(annotation.Description);
                }
            }
            return 0;
        }
        // [END analyze_labels_gcs]

        // [START analyze_faces]
        public static object AnalyzeFaces(string path)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputContent = Convert.ToBase64String(File.ReadAllBytes(path)),
                Features = { Feature.FaceDetection }
            };
            var op = client.AnnotateVideo(request).PollUntilCompleted();
            char faceLabel = 'A';
            foreach (var result in op.Result.AnnotationResults)
            {
                foreach (var annotation in result.FaceAnnotations)
                {
                    Console.WriteLine("Face {0} seen at offsets:", faceLabel);
                    foreach (var segment in annotation.Segments)
                    {
                        Console.WriteLine("{0}-{1}", segment.StartTimeOffset, segment.EndTimeOffset);
                    }
                    ++faceLabel;
                }
            }
            return 0;
        }
        // [END analyze_faces]

        // [START analyze_faces_gcs]
        public static object AnalyzeFacesGcs(string uri)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputUri = uri,
                Features = { Feature.FaceDetection }
            };
            var op = client.AnnotateVideo(request).PollUntilCompleted();
            char faceLabel = 'A';
            foreach (var result in op.Result.AnnotationResults)
            {
                foreach (var annotation in result.FaceAnnotations)
                {
                    Console.WriteLine("Face {0} seen at offsets:", faceLabel);
                    foreach (var segment in annotation.Segments)
                    {
                        Console.WriteLine("{0}-{1}", segment.StartTimeOffset, segment.EndTimeOffset);
                    }
                    ++faceLabel;
                }
            }
            return 0;
        }
        // [END analyze_faces_gcs]

        public static void Main(string[] args)
        {
            // TODO: add faces command when it becomes publicly available.
            Parser.Default.ParseArguments<
                AnalyzeShotsOptions,
                // AnalyzeFacesOptions,
                AnalyzeLabelsOptions
                >(args).MapResult(
                (AnalyzeShotsOptions opts) => IsStorageUri(opts.Uri) ? AnalyzeShotsGcs(opts.Uri) : AnalyzeShots(opts.Uri),
                // (AnalyzeFacesOptions opts) => IsStorageUri(opts.Uri) ? AnalyzeFacesGcs(opts.Uri) : AnalyzeFaces(opts.Uri),
                (AnalyzeLabelsOptions opts) => IsStorageUri(opts.Uri) ? AnalyzeLabelsGcs(opts.Uri) : AnalyzeLabels(opts.Uri),
                errs => 1);
        }
        static bool IsStorageUri(string s) => s.Substring(0, 4).ToLower() == "gs:/";
    }
}
