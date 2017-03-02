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
    class CloudStorageOptions
    {
        [Value(0,
            HelpText = "The name of the Google Cloud Storage bucket that contains the image to be examined.",
            Required = false)]
        public string BucketName { get; set; }

        [Value(1, HelpText = "The object name.", Required = true)]
        public string ObjectName { get; set; }
    }

    class LocalOptions
    {
        [Value(0, HelpText = "The path to the image file.", Required = true)]
        public string FilePath { get; set; }
    }

    [Verb("labels", HelpText = "Detect labels in a local image file.")]
    class DetectLocalLabelsOptions : LocalOptions { }

    [Verb("labels-gcs", HelpText = "Detect labels in an image stored in Google Cloud Storage.")]
    class DetectCloudStorageLabelsOptions : CloudStorageOptions { }

    [Verb("safe-search", HelpText = "Detect safe-search in a local image file.")]
    class DetectLocalSafeSearchOptions : LocalOptions { }

    [Verb("safe-search-gcs", HelpText = "Detect safe-search in an image stored in Google Cloud Storage.")]
    class DetectCloudStorageSafeSearchOptions : CloudStorageOptions { }

    [Verb("properties", HelpText = "Detect properties in a local image file.")]
    class DetectLocalPropertiesOptions : LocalOptions { }

    [Verb("properties-gcs", HelpText = "Detect properties in an image stored in Google Cloud Storage.")]
    class DetectCloudStoragePropertiesOptions : CloudStorageOptions { }

    [Verb("faces", HelpText = "Detect faces in a local image file.")]
    class DetectLocalFacesOptions : LocalOptions { }

    [Verb("faces-gcs", HelpText = "Detect faces in an image stored in Google Cloud Storage.")]
    class DetectCloudStorageFacesOptions : CloudStorageOptions { }

    [Verb("landmarks", HelpText = "Detect landmarks in a local image file.")]
    class DetectLocalLandmarksOptions : LocalOptions { }

    [Verb("landmarks-gcs", HelpText = "Detect landmarks in an image stored in Google Cloud Storage.")]
    class DetectCloudStorageLandmarksOptions : CloudStorageOptions { }

    [Verb("text", HelpText = "Detect text in a local image file.")]
    class DetectLocalTextOptions : LocalOptions { }

    [Verb("text-gcs", HelpText = "Detect text in an image stored in Google Cloud Storage.")]
    class DetectCloudStorageTextOptions : CloudStorageOptions { }

    [Verb("logos", HelpText = "Detect logos in a local image file.")]
    class DetectLocalLogosOptions : LocalOptions { }

    [Verb("logos-gcs", HelpText = "Detect logos in an image stored in Google Cloud Storage.")]
    class DetectCloudStorageLogosOptions : CloudStorageOptions { }


    public class DetectProgram
    {
        // [START vision_face_detection_gcs]
        private static object DetectFaces(string bucketName, string objectName)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromUri($"gs://{bucketName}/{objectName}");
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
            return 0;
        }
        // [END vision_face_detection_gcs]

        // [START vision_face_detection]
        private static object DetectFaces(string filePath)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(filePath);
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
            return 0;
        }
        // [END vision_face_detection]

        // [START vision_label_detection_gcs]
        private static object DetectLabels(string bucketName, string objectName)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromUri($"gs://{bucketName}/{objectName}");
            var response = client.DetectLabels(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            return 0;
        }
        // [END vision_label_detection_gcs]

        // [START vision_label_detection]
        private static object DetectLabels(string filePath)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(filePath);
            var response = client.DetectLabels(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            return 0;
        }
        // [END vision_label_detection]

        // [START vision_safe_search_detection_gcs]
        private static object DetectSafeSearch(string bucketName, string objectName)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromUri($"gs://{bucketName}/{objectName}");
            var response = client.DetectSafeSearch(image);
            Console.WriteLine("Adult: {0}", response.Adult.ToString());
            Console.WriteLine("Spoof: {0}", response.Spoof.ToString());
            Console.WriteLine("Medical: {0}", response.Medical.ToString());
            Console.WriteLine("Violence: {0}", response.Violence.ToString());
            return 0;
        }
        // [END vision_safe_search_detection_gcs]

        // [START vision_safe_search_detection]
        private static object DetectSafeSearch(string filePath)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(filePath);
            var response = client.DetectSafeSearch(image);
            Console.WriteLine("Adult: {0}", response.Adult.ToString());
            Console.WriteLine("Spoof: {0}", response.Spoof.ToString());
            Console.WriteLine("Medical: {0}", response.Medical.ToString());
            Console.WriteLine("Violence: {0}", response.Violence.ToString());
            return 0;
        }
        // [END vision_safe_search_detection]

        // [START vision_image_property_detection_gcs]
        private static object DetectProperties(string bucketName, string objectName)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromUri($"gs://{bucketName}/{objectName}");
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
            return 0;
        }
        // [END vision_image_property_detection_gcs]

        // [START vision_image_property_detection]
        private static object DetectProperties(string filePath)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(filePath);
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
            return 0;
        }
        // [END vision_image_property_detection]

        // [START vision_landmark_detection_gcs]
        private static object DetectLandmarks(string bucketName, string objectName)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromUri($"gs://{bucketName}/{objectName}");
            var response = client.DetectLandmarks(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            return 0;
        }
        // [END vision_landmark_detection_gcs]

        // [START vision_landmark_detection]
        private static object DetectLandmarks(string filePath)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(filePath);
            var response = client.DetectLandmarks(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            return 0;
        }
        // [END vision_landmark_detection]

        // [START vision_text_detection_gcs]
        private static object DetectText(string bucketName, string objectName)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromUri($"gs://{bucketName}/{objectName}");
            var response = client.DetectText(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            return 0;
        }
        // [END vision_text_detection_gcs]

        // [START vision_text_detection]
        private static object DetectText(string filePath)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(filePath);
            var response = client.DetectText(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            return 0;
        }
        // [END vision_text_detection]

        // [START vision_logo_detection_gcs]
        private static object DetectLogos(string bucketName, string objectName)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromUri($"gs://{bucketName}/{objectName}");
            var response = client.DetectLogos(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            return 0;
        }
        // [END vision_logo_detection_gcs]

        // [START vision_logo_detection]
        private static object DetectLogos(string filePath)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(filePath);
            var response = client.DetectLogos(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            return 0;
        }
        // [END vision_logo_detection]

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                DetectLocalLabelsOptions, DetectCloudStorageLabelsOptions,
                DetectLocalSafeSearchOptions, DetectCloudStorageSafeSearchOptions,
                DetectLocalPropertiesOptions, DetectCloudStoragePropertiesOptions,
                DetectLocalFacesOptions, DetectCloudStorageFacesOptions,
                DetectLocalTextOptions, DetectCloudStorageTextOptions,
                DetectLocalLogosOptions, DetectCloudStorageLogosOptions,
                DetectLocalLandmarksOptions, DetectCloudStorageLandmarksOptions
                >(args)
              .MapResult(
                (DetectLocalLabelsOptions opts) => DetectLabels(opts.FilePath),
                (DetectCloudStorageLabelsOptions opts) => DetectLabels(opts.BucketName, opts.ObjectName),
                (DetectLocalSafeSearchOptions opts) => DetectSafeSearch(opts.FilePath),
                (DetectCloudStorageSafeSearchOptions opts) => DetectSafeSearch(opts.BucketName, opts.ObjectName),
                (DetectLocalPropertiesOptions opts) => DetectProperties(opts.FilePath),
                (DetectCloudStoragePropertiesOptions opts) => DetectProperties(opts.BucketName, opts.ObjectName),
                (DetectLocalFacesOptions opts) => DetectFaces(opts.FilePath),
                (DetectCloudStorageFacesOptions opts) => DetectFaces(opts.BucketName, opts.ObjectName),
                (DetectLocalLandmarksOptions opts) => DetectLandmarks(opts.FilePath),
                (DetectCloudStorageLandmarksOptions opts) => DetectLandmarks(opts.BucketName, opts.ObjectName),
                (DetectLocalTextOptions opts) => DetectText(opts.FilePath),
                (DetectCloudStorageTextOptions opts) => DetectText(opts.BucketName, opts.ObjectName),
                (DetectLocalLogosOptions opts) => DetectLogos(opts.FilePath),
                (DetectCloudStorageLogosOptions opts) => DetectLogos(opts.BucketName, opts.ObjectName),
                errs => 1);
        }
    }
}
