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
using Google.Cloud.Storage.V1;
using Google.Cloud.Vision.V1;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoogleCloudSamples
{
    class ImageOptions
    {
        [Value(0, HelpText = "The path to the image file. " +
            "May be a local file path, a Google Cloud Storage uri, or " +
            "a publicly accessible HTTP or HTTPS uri",
            Required = true)]
        public string FilePath { get; set; }
    }

    [Verb("faces", HelpText = "Detect faces.")]
    class DetectFacesOptions : ImageOptions { }

    public class DetectProgram
    {
        static Image ImageFromArg(string arg)
        {
            bool isFromUri = false;
            if (arg.ToLower().StartsWith("gs:/") ||
                arg.ToLower().StartsWith("http"))
            {
                isFromUri = true;
            }
            return isFromUri ? ImageFromUri(arg) : ImageFromFile(arg);
        }

        static Image ImageFromUri(string uri)
        {
            // [START vision_face_detection_gcs]
            // Specify a Google Cloud Storage uri for the image
            // or a publicly accessible HTTP or HTTPS uri.
            var image = Image.FromUri(uri);
            // [END vision_face_detection_gcs]
            return image;
        }

        static Image ImageFromFile(string filePath)
        {
            // [START vision_face_detection]
            // Load an image from a local file.
            var image = Image.FromFile(filePath);
            // [END vision_face_detection]
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

        private static object DetectTextWithLocation(Image image)
        {
            // [START vision_set_endpoint]
            // Instantiate a client connected to the 'eu' location.
            var client = new ImageAnnotatorClientBuilder
            {
                Endpoint = "eu-vision.googleapis.com"
            }.Build();
            // [END vision_set_endpoint]
            var response = client.DetectText(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            return 0;
        }


        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                DetectFacesOptions
                >(args)
              .MapResult(
                (DetectFacesOptions opts) => DetectFaces(ImageFromArg(opts.FilePath)),
                errs => 1);
        }
    }
}
