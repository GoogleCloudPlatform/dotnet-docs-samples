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
    class VideoOptions
    {
        [Value(0, HelpText = "The uri of the video to examine. "
            + "Can be path to a local file or a Cloud storage uri like "
            + "gs://bucket/object.",
            Required = true)]
        public string Uri { get; set; }
    }

    class StorageOnlyVideoOptions
    {
        [Value(0, HelpText = "The uri of the video to examine. "
            + "Must be a Cloud storage uri like "
            + "gs://bucket/object.",
            Required = true)]
        public string Uri { get; set; }
    }

    [Verb("labels", HelpText = "Print a list of labels found in the video.")]
    class AnalyzeLabelsOptions : VideoOptions
    { }

    [Verb("shots", HelpText = "Print a list shot changes.")]
    class AnalyzeShotsOptions : StorageOnlyVideoOptions
    { }

    [Verb("explicit-content", HelpText = "Analyze the content of the video.")]
    class AnalyzeExplicitContentOptions : StorageOnlyVideoOptions
    { }

    [Verb("transcribe", HelpText = "Print the audio track as text")]
    class TranscribeOptions : StorageOnlyVideoOptions
    { }

    [Verb("text", HelpText = "Detect text in a video")]
    class DetectTextOptions : VideoOptions
    { }

    [Verb("track-object", HelpText = "Track objects in a video")]
    class TrackObjectOptions : VideoOptions
    { }

    [Verb("logo-detect", HelpText = "Detect logos in a video")]
    class DetectLogoOptions : VideoOptions
    { }

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
                Features = { Feature.LabelDetection },
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

        // [START video_speech_transcription_gcs]
        public static object TranscribeVideo(string uri)
        {
            Console.WriteLine("Processing video for speech transcription.");

            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest
            {
                InputUri = uri,
                Features = { Feature.SpeechTranscription },
                VideoContext = new VideoContext
                {
                    SpeechTranscriptionConfig = new SpeechTranscriptionConfig
                    {
                        LanguageCode = "en-US",
                        EnableAutomaticPunctuation = true
                    }
                },
            };
            var op = client.AnnotateVideo(request).PollUntilCompleted();

            // There is only one annotation result since only one video is
            // processed.
            var annotationResults = op.Result.AnnotationResults[0];
            foreach (var transcription in annotationResults.SpeechTranscriptions)
            {
                // The number of alternatives for each transcription is limited
                // by SpeechTranscriptionConfig.MaxAlternatives.
                // Each alternative is a different possible transcription
                // and has its own confidence score.
                foreach (var alternative in transcription.Alternatives)
                {
                    Console.WriteLine("Alternative level information:");

                    Console.WriteLine($"Transcript: {alternative.Transcript}");
                    Console.WriteLine($"Confidence: {alternative.Confidence}");

                    foreach (var wordInfo in alternative.Words)
                    {
                        Console.WriteLine($"\t{wordInfo.StartTime} - " +
                                          $"{wordInfo.EndTime}:" +
                                          $"{wordInfo.Word}");
                    }
                }
            }

            return 0;
        }
        // [END video_speech_transcription_gcs]

        // [START video_detect_logo]
        public static object DetectLogo(string filePath)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputContent = Google.Protobuf.ByteString.CopyFrom(File.ReadAllBytes(filePath)),
                Features = { Feature.LogoRecognition }
            };

            Console.WriteLine("\nWaiting for operation to complete...");
            var op = client.AnnotateVideo(request).PollUntilCompleted();

            // The first result is retrieved because a single video was processed.
            var annotationResults = op.Result.AnnotationResults[0];

            // Annotations for list of logos detected, tracked and recognized in video.
            foreach (var logoRecognitionAnnotation in annotationResults.LogoRecognitionAnnotations)
            {
                var entity = logoRecognitionAnnotation.Entity;
                // Opaque entity ID. Some IDs may be available in
                // [Google Knowledge Graph Search API](https://developers.google.com/knowledge-graph/).
                Console.WriteLine($"Entity ID :{entity.EntityId}");
                Console.WriteLine($"Description :{entity.Description}");

                // All logo tracks where the recognized logo appears. Each track corresponds to one logo
                // instance appearing in consecutive frames.
                foreach (var track in logoRecognitionAnnotation.Tracks)
                {
                    // Video segment of a track.
                    var startTimeOffset = track.Segment.StartTimeOffset;
                    Console.WriteLine(
                        $"Start Time Offset: {startTimeOffset.Seconds}.{startTimeOffset.Nanos}");
                    var endTimeOffset = track.Segment.EndTimeOffset;
                    Console.WriteLine(
                        $"End Time Offset: {endTimeOffset.Seconds}.{endTimeOffset.Seconds}");
                    Console.WriteLine($"Confidence: {track.Confidence}");

                    // The object with timestamp and attributes per frame in the track.
                    foreach (var timestampedObject in track.TimestampedObjects)
                    {
                        // Normalized Bounding box in a frame, where the object is located.
                        var normalizedBoundingBox = timestampedObject.NormalizedBoundingBox;
                        Console.WriteLine($"Left: {normalizedBoundingBox.Left}");
                        Console.WriteLine($"Top: {normalizedBoundingBox.Top}");
                        Console.WriteLine($"Right: {normalizedBoundingBox.Right}");
                        Console.WriteLine($"Bottom: {normalizedBoundingBox.Bottom}");

                        // Optional. The attributes of the object in the bounding box.
                        foreach (var attribute in timestampedObject.Attributes)
                        {
                            Console.WriteLine($"Name: {attribute.Name}");
                            Console.WriteLine($"Confidence: {attribute.Confidence}");
                            Console.WriteLine($"Value: {attribute.Value}");
                        }

                        // Optional. Attributes in the track level.
                        foreach (var trackAttribute in track.Attributes)
                        {
                            Console.WriteLine($"Name : {trackAttribute.Name}");
                            Console.WriteLine($"Confidence : {trackAttribute.Confidence}");
                            Console.WriteLine($"Value : {trackAttribute.Value}");
                        }
                    }

                    // All video segments where the recognized logo appears. There might be multiple instances
                    // of the same logo class appearing in one VideoSegment.
                    foreach (var segment in logoRecognitionAnnotation.Segments)
                    {
                        Console.WriteLine(
                            $"Start Time Offset : {segment.StartTimeOffset.Seconds}.{segment.StartTimeOffset.Nanos}");
                        Console.WriteLine(
                            $"End Time Offset : {segment.EndTimeOffset.Seconds}.{segment.EndTimeOffset.Nanos}");
                    }
                }
            }
            return 0;
        }
        // [END video_detect_logo]

        // [START video_detect_logo_gcs]
        public static object DetectLogoGcs(string gcsUri)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest()
            {
                InputUri = gcsUri,
                Features = { Feature.LogoRecognition }
            };

            Console.WriteLine("\nWaiting for operation to complete...");
            var op = client.AnnotateVideo(request).PollUntilCompleted();

            // The first result is retrieved because a single video was processed.
            var annotationResults = op.Result.AnnotationResults[0];

            // Annotations for list of logos detected, tracked and recognized in video.
            foreach (var logoRecognitionAnnotation in annotationResults.LogoRecognitionAnnotations)
            {
                var entity = logoRecognitionAnnotation.Entity;
                // Opaque entity ID. Some IDs may be available in
                // [Google Knowledge Graph Search API](https://developers.google.com/knowledge-graph/).
                Console.WriteLine($"Entity ID :{entity.EntityId}");
                Console.WriteLine($"Description :{entity.Description}");

                // All logo tracks where the recognized logo appears. Each track corresponds to one logo
                // instance appearing in consecutive frames.
                foreach (var track in logoRecognitionAnnotation.Tracks)
                {
                    // Video segment of a track.
                    var startTimeOffset = track.Segment.StartTimeOffset;
                    Console.WriteLine(
                        $"Start Time Offset: {startTimeOffset.Seconds}.{startTimeOffset.Nanos}");
                    var endTimeOffset = track.Segment.EndTimeOffset;
                    Console.WriteLine(
                        $"End Time Offset: {endTimeOffset.Seconds}.{endTimeOffset.Seconds}");
                    Console.WriteLine($"\tConfidence: {track.Confidence}");

                    // The object with timestamp and attributes per frame in the track.
                    foreach (var timestampedObject in track.TimestampedObjects)
                    {
                        // Normalized Bounding box in a frame, where the object is located.
                        var normalizedBoundingBox = timestampedObject.NormalizedBoundingBox;
                        Console.WriteLine($"Left: {normalizedBoundingBox.Left}");
                        Console.WriteLine($"Top: {normalizedBoundingBox.Top}");
                        Console.WriteLine($"Right: {normalizedBoundingBox.Right}");
                        Console.WriteLine($"Bottom: {normalizedBoundingBox.Bottom}");

                        // Optional. The attributes of the object in the bounding box.
                        foreach (var attribute in timestampedObject.Attributes)
                        {
                            Console.WriteLine($"Name: {attribute.Name}");
                            Console.WriteLine($"Confidence: {attribute.Confidence}");
                            Console.WriteLine($"Value: {attribute.Value}");
                        }

                        // Optional. Attributes in the track level.
                        foreach (var trackAttribute in track.Attributes)
                        {
                            Console.WriteLine($"Name : {trackAttribute.Name}");
                            Console.WriteLine($"Confidence : {trackAttribute.Confidence}");
                            Console.WriteLine($"Value : {trackAttribute.Value}");
                        }
                    }

                    // All video segments where the recognized logo appears. There might be multiple instances
                    // of the same logo class appearing in one VideoSegment.
                    foreach (var segment in logoRecognitionAnnotation.Segments)
                    {
                        Console.WriteLine(
                            $"Start Time Offset : {segment.StartTimeOffset.Seconds}.{segment.StartTimeOffset.Nanos}");
                        Console.WriteLine(
                            $"End Time Offset : {segment.EndTimeOffset.Seconds}.{segment.EndTimeOffset.Nanos}");
                    }
                }
            }
            return 0;
        }
        // [END video_detect_logo_gcs]

        // [START video_detect_text_gcs]
        public static object DetectTextGcs(string gcsUri)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest
            {
                InputUri = gcsUri,
                Features = { Feature.TextDetection },
            };

            Console.WriteLine("\nProcessing video for text detection.");
            var op = client.AnnotateVideo(request).PollUntilCompleted();

            // Retrieve the first result because only one video was processed.
            var annotationResults = op.Result.AnnotationResults[0];

            // Get only the first result.
            var textAnnotation = annotationResults.TextAnnotations[0];
            Console.WriteLine($"\nText: {textAnnotation.Text}");

            // Get the first text segment.
            var textSegment = textAnnotation.Segments[0];
            var startTime = textSegment.Segment.StartTimeOffset;
            var endTime = textSegment.Segment.EndTimeOffset;
            Console.Write(
                $"Start time: {startTime.Seconds + startTime.Nanos / 1e9 }, ");
            Console.WriteLine(
                $"End time: {endTime.Seconds + endTime.Nanos / 1e9 }");

            Console.WriteLine($"Confidence: {textSegment.Confidence}");

            // Show the result for the first frame in this segment.
            var frame = textSegment.Frames[0];
            var timeOffset = frame.TimeOffset;
            Console.Write("Time offset for the first frame: ");
            Console.WriteLine(timeOffset.Seconds + timeOffset.Nanos * 1e9);
            Console.WriteLine("Rotated Bounding Box Vertices:");
            foreach (var vertex in frame.RotatedBoundingBox.Vertices)
            {
                Console.WriteLine(
                    $"\tVertex x: {vertex.X}, Vertex.y: {vertex.Y}");
            }
            return 0;
        }
        // [END video_detect_text_gcs]

        // [START video_detect_text]
        public static object DetectText(string filePath)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest
            {
                InputContent = Google.Protobuf.ByteString.CopyFrom(File.ReadAllBytes(filePath)),
                Features = { Feature.TextDetection },
            };

            Console.WriteLine("\nProcessing video for text detection.");
            var op = client.AnnotateVideo(request).PollUntilCompleted();

            // Retrieve the first result because only one video was processed.
            var annotationResults = op.Result.AnnotationResults[0];

            // Get only the first result.
            var textAnnotation = annotationResults.TextAnnotations[0];
            Console.WriteLine($"\nText: {textAnnotation.Text}");

            // Get the first text segment.
            var textSegment = textAnnotation.Segments[0];
            var startTime = textSegment.Segment.StartTimeOffset;
            var endTime = textSegment.Segment.EndTimeOffset;
            Console.Write(
                $"Start time: {startTime.Seconds + startTime.Nanos / 1e9 }, ");
            Console.WriteLine(
                $"End time: {endTime.Seconds + endTime.Nanos / 1e9 }");

            Console.WriteLine($"Confidence: {textSegment.Confidence}");

            // Show the result for the first frame in this segment.
            var frame = textSegment.Frames[0];
            var timeOffset = frame.TimeOffset;
            Console.Write("Time offset for the first frame: ");
            Console.WriteLine(timeOffset.Seconds + timeOffset.Nanos * 1e9);
            Console.WriteLine("Rotated Bounding Box Vertices:");
            foreach (var vertex in frame.RotatedBoundingBox.Vertices)
            {
                Console.WriteLine(
                    $"\tVertex x: {vertex.X}, Vertex.y: {vertex.Y}");
            }
            return 0;
        }
        // [END video_detect_text]

        // [START video_object_tracking_gcs]
        public static object TrackObjectGcs(string gcsUri)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest
            {
                InputUri = gcsUri,
                Features = { Feature.ObjectTracking },
                // It is recommended to use location_id as 'us-east1' for the
                // best latency due to different types of processors used in
                // this region and others.
                LocationId = "us-east1"
            };

            Console.WriteLine("\nProcessing video for object annotations.");
            var op = client.AnnotateVideo(request).PollUntilCompleted();

            Console.WriteLine("\nFinished processing.\n");

            // Retrieve first result because a single video was processed.
            var objectAnnotations = op.Result.AnnotationResults[0]
                                      .ObjectAnnotations;

            // Get only the first annotation for demo purposes
            var objAnnotation = objectAnnotations[0];

            Console.WriteLine(
                $"Entity description: {objAnnotation.Entity.Description}");

            if (objAnnotation.Entity.EntityId != null)
            {
                Console.WriteLine(
                    $"Entity id: {objAnnotation.Entity.EntityId}");
            }

            Console.Write($"Segment: ");
            Console.WriteLine(
                String.Format("{0}s to {1}s",
                              objAnnotation.Segment.StartTimeOffset.Seconds +
                              objAnnotation.Segment.StartTimeOffset.Nanos / 1e9,
                              objAnnotation.Segment.EndTimeOffset.Seconds +
                              objAnnotation.Segment.EndTimeOffset.Nanos / 1e9));

            Console.WriteLine($"Confidence: {objAnnotation.Confidence}");

            // Here we print only the bounding box of the first frame in this segment
            var frame = objAnnotation.Frames[0];
            var box = frame.NormalizedBoundingBox;
            Console.WriteLine(
                String.Format("Time offset of the first frame: {0}s",
                              frame.TimeOffset.Seconds +
                              frame.TimeOffset.Nanos / 1e9));
            Console.WriteLine("Bounding box positions:");
            Console.WriteLine($"\tleft   : {box.Left}");
            Console.WriteLine($"\ttop    : {box.Top}");
            Console.WriteLine($"\tright  : {box.Right}");
            Console.WriteLine($"\tbottom : {box.Bottom}");

            return 0;
        }
        // [END video_object_tracking_gcs]

        // [START video_object_tracking]
        public static object TrackObject(string filePath)
        {
            var client = VideoIntelligenceServiceClient.Create();
            var request = new AnnotateVideoRequest
            {
                InputContent = Google.Protobuf.ByteString.CopyFrom(File.ReadAllBytes(filePath)),
                Features = { Feature.ObjectTracking },
                // It is recommended to use location_id as 'us-east1' for the
                // best latency due to different types of processors used in
                // this region and others.
                LocationId = "us-east1"
            };

            Console.WriteLine("\nProcessing video for object annotations.");
            var op = client.AnnotateVideo(request).PollUntilCompleted();

            Console.WriteLine("\nFinished processing.\n");

            // Retrieve first result because a single video was processed.
            var objectAnnotations = op.Result.AnnotationResults[0]
                                      .ObjectAnnotations;

            // Get only the first annotation for demo purposes
            var objAnnotation = objectAnnotations[0];

            Console.WriteLine(
                $"Entity description: {objAnnotation.Entity.Description}");

            if (objAnnotation.Entity.EntityId != null)
            {
                Console.WriteLine(
                    $"Entity id: {objAnnotation.Entity.EntityId}");
            }

            Console.Write($"Segment: ");
            Console.WriteLine(
                String.Format("{0}s to {1}s",
                              objAnnotation.Segment.StartTimeOffset.Seconds +
                              objAnnotation.Segment.StartTimeOffset.Nanos / 1e9,
                              objAnnotation.Segment.EndTimeOffset.Seconds +
                              objAnnotation.Segment.EndTimeOffset.Nanos / 1e9));

            Console.WriteLine($"Confidence: {objAnnotation.Confidence}");

            // Here we print only the bounding box of the first frame in this segment
            var frame = objAnnotation.Frames[0];
            var box = frame.NormalizedBoundingBox;
            Console.WriteLine(
                String.Format("Time offset of the first frame: {0}s",
                              frame.TimeOffset.Seconds +
                              frame.TimeOffset.Nanos / 1e9));
            Console.WriteLine("Bounding box positions:");
            Console.WriteLine($"\tleft   : {box.Left}");
            Console.WriteLine($"\ttop    : {box.Top}");
            Console.WriteLine($"\tright  : {box.Right}");
            Console.WriteLine($"\tbottom : {box.Bottom}");

            return 0;
        }
        // [END video_object_tracking]

        public static void Main(string[] args)
        {
            var verbMap = new VerbMap<object>()
                .Add((AnalyzeShotsOptions opts) => AnalyzeShotsGcs(opts.Uri))
                .Add((AnalyzeExplicitContentOptions opts) => AnalyzeExplicitContentGcs(opts.Uri))
                .Add((AnalyzeLabelsOptions opts) => IsStorageUri(opts.Uri) ? AnalyzeLabelsGcs(opts.Uri) : AnalyzeLabels(opts.Uri))
                .Add((TranscribeOptions opts) => TranscribeVideo(opts.Uri))
                .Add((DetectTextOptions opts) => IsStorageUri(opts.Uri) ? DetectTextGcs(opts.Uri) : DetectText(opts.Uri))
                .Add((TrackObjectOptions opts) => IsStorageUri(opts.Uri) ? TrackObjectGcs(opts.Uri) : TrackObject(opts.Uri))
                .Add((DetectLogoOptions opts) => IsStorageUri(opts.Uri) ? DetectLogoGcs(opts.Uri) : DetectLogo(opts.Uri))
                .SetNotParsedFunc((errs) => 1);
            verbMap.Run(args);
        }
        static bool IsStorageUri(string s) => s.Substring(0, 4).ToLower() == "gs:/";
    }
}
