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
using Google.Cloud.Vision.V1;
using System;

namespace GoogleCloudSamples
{
    class ImageOptions
    {
        [Value(0, HelpText = "The path to the image file. " +
            "May be a local file path or a Google Cloud Storage uri.",
            Required = true)]
        public string FilePath { get; set; }
    }

    [Verb("labels", HelpText = "Detect labels.")]
    class DetectLabelsOptions : ImageOptions { }

    [Verb("safe-search", HelpText = "Detect safe-search.")]
    class DetectSafeSearchOptions : ImageOptions { }

    [Verb("properties", HelpText = "Detect properties.")]
    class DetectPropertiesOptions : ImageOptions { }

    [Verb("faces", HelpText = "Detect faces.")]
    class DetectFacesOptions : ImageOptions { }

    [Verb("landmarks", HelpText = "Detect landmarks.")]
    class DetectLandmarksOptions : ImageOptions { }

    [Verb("text", HelpText = "Detect text.")]
    class DetectTextOptions : ImageOptions { }

    [Verb("logos", HelpText = "Detect logos.")]
    class DetectLogosOptions : ImageOptions { }


    public class DetectProgram
    {
        static Image ImageFromArg(string arg)
        {
            return arg.ToLower().StartsWith("gs:/") ?
                ImageFromUri(arg) : ImageFromFile(arg);
        }

        static Image ImageFromUri(string uri)
        {
            // [START vision_logo_detection_gcs]
            // [START vision_text_detection_gcs]
            // [START vision_landmark_detection_gcs]
            // [START vision_image_property_detection_gcs]
            // [START vision_safe_search_detection_gcs]
            // [START vision_label_detection_gcs]
            // [START vision_face_detection_gcs]
            // Specify a Google Cloud Storage uri for the image.
            var image = Image.FromUri(uri);
            // [END vision_face_detection_gcs]
            // [END vision_label_detection_gcs]
            // [END vision_safe_search_detection_gcs]
            // [END vision_image_property_detection_gcs]
            // [END vision_landmark_detection_gcs]
            // [END vision_text_detection_gcs]
            // [END vision_logo_detection_gcs]
            return image;
        }

        static Image ImageFromFile(string filePath)
        {
            // [START vision_logo_detection]
            // [START vision_text_detection]
            // [START vision_landmark_detection]
            // [START vision_image_property_detection]
            // [START vision_safe_search_detection]
            // [START vision_label_detection]
            // [START vision_face_detection]
            // Load an image from a local file.
            var image = Image.FromFile(filePath);
            // [END vision_face_detection]
            // [END vision_label_detection]
            // [END vision_safe_search_detection]
            // [END vision_image_property_detection]
            // [END vision_landmark_detection]
            // [END vision_text_detection]
            // [END vision_logo_detection]
            return image;
        }

        private static object DetectFaces(Image image)
        {
            // [START vision_face_detection]
            // [START vision_face_detection_gcs]
            var client = ImageAnnotatorClient.Create();
            var response = client.DetectFaces(image);
            int count = 1;
            foreach (var faceAnnotation in response)
            {
                Console.WriteLine("Face {0}:", count++);
                Console.WriteLine("  Joy: {0}", faceAnnotation.JoyLikelihood);
                Console.WriteLine("  Anger: {0}", faceAnnotation.AngerLikelihood);
                Console.WriteLine("  Sorrow: {0}", faceAnnotation.SorrowLikelihood);
                Console.WriteLine("  Surprise: {0}", faceAnnotation.SurpriseLikelihood);
            }
            // [END vision_face_detection_gcs]
            // [END vision_face_detection]
            return 0;
        }

        private static object DetectLabels(Image image)
        {
            // [START vision_label_detection]
            // [START vision_label_detection_gcs]
            var client = ImageAnnotatorClient.Create();
            var response = client.DetectLabels(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            // [END vision_label_detection_gcs]
            // [END vision_label_detection]
            return 0;
        }

        private static object DetectSafeSearch(Image image)
        {
            // [START vision_safe_search_detection]
            // [START vision_safe_search_detection_gcs]
            var client = ImageAnnotatorClient.Create();
            var response = client.DetectSafeSearch(image);
            Console.WriteLine("Adult: {0}", response.Adult.ToString());
            Console.WriteLine("Spoof: {0}", response.Spoof.ToString());
            Console.WriteLine("Medical: {0}", response.Medical.ToString());
            Console.WriteLine("Violence: {0}", response.Violence.ToString());
            // [END vision_safe_search_detection_gcs]
            // [END vision_safe_search_detection]
            return 0;
        }

        private static object DetectProperties(Image image)
        {
            // [START vision_image_property_detection]
            // [START vision_image_property_detection_gcs]
            var client = ImageAnnotatorClient.Create();
            var response = client.DetectImageProperties(image);
            string header = "Red\tGreen\tBlue\tAlpha\n";
            foreach (var color in response.DominantColors.Colors)
            {
                Console.Write(header);
                header = "";
                Console.WriteLine("{0}\t{0}\t{0}\t{0}",
                    color.Color.Red, color.Color.Green, color.Color.Blue,
                    color.Color.Alpha);
            }
            // [END vision_image_property_detection_gcs]
            // [END vision_image_property_detection]
            return 0;
        }

        private static object DetectLandmarks(Image image)
        {
            // [START vision_landmark_detection]
            // [START vision_landmark_detection_gcs]
            var client = ImageAnnotatorClient.Create();
            var response = client.DetectLandmarks(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            // [END vision_landmark_detection_gcs]
            // [END vision_landmark_detection]
            return 0;
        }

        private static object DetectText(Image image)
        {
            // [START vision_text_detection]
            // [START vision_text_detection_gcs]
            var client = ImageAnnotatorClient.Create();
            var response = client.DetectText(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            // [END vision_text_detection_gcs]
            // [END vision_text_detection]
            return 0;
        }


        private static object DetectLogos(Image image)
        {
            // [START vision_logo_detection]
            // [START vision_logo_detection_gcs]
            var client = ImageAnnotatorClient.Create();
            var response = client.DetectLogos(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            // [END vision_logo_detection_gcs]
            // [END vision_logo_detection]
            return 0;
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                DetectLabelsOptions,
                DetectSafeSearchOptions,
                DetectPropertiesOptions,
                DetectFacesOptions,
                DetectTextOptions,
                DetectLogosOptions,
                DetectLandmarksOptions
                >(args)
              .MapResult(
                (DetectLabelsOptions opts) => DetectLabels(ImageFromArg(opts.FilePath)),
                (DetectSafeSearchOptions opts) => DetectSafeSearch(ImageFromArg(opts.FilePath)),
                (DetectPropertiesOptions opts) => DetectProperties(ImageFromArg(opts.FilePath)),
                (DetectFacesOptions opts) => DetectFaces(ImageFromArg(opts.FilePath)),
                (DetectLandmarksOptions opts) => DetectLandmarks(ImageFromArg(opts.FilePath)),
                (DetectTextOptions opts) => DetectText(ImageFromArg(opts.FilePath)),
                (DetectLogosOptions opts) => DetectLogos(ImageFromArg(opts.FilePath)),
                errs => 1);
        }
    }
}
