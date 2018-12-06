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
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Google.Protobuf;

namespace GoogleCloudSamples
{
    [Verb("create", HelpText = "Build a product set")]
    class ProductOptions
    {
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }

        [Value(1, HelpText = "Region name")]
        public string ComputeRegion { get; set; }

        [Value(2, HelpText = "Product Set ID")]
        public string ProductSetId { get; set; }

        [Value(3, HelpText = "Product Set Display Name")]
        public string ProductSetDisplayName { get; set; }
    }

    [Verb("list", HelpText = "List all product sets")]
    class ListProductOptions
    { 
        
    
    }


    class ImageOptions
    {
        [Value(0, HelpText = "The path to the image file. " +
            "May be a local file path, a Google Cloud Storage uri, or " +
            "a publicly accessible HTTP or HTTPS uri",
            Required = true)]
        public string FilePath { get; set; }
    }

    public class ProductSearchProgram
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
            // [START vision_crop_hint_detection_gcs]
            // [START vision_fulltext_detection_gcs]
            // [START vision_web_detection_gcs]
            // [START vision_logo_detection_gcs]
            // [START vision_text_detection_gcs]
            // [START vision_landmark_detection_gcs]
            // [START vision_image_property_detection_gcs]
            // [START vision_safe_search_detection_gcs]
            // [START vision_label_detection_gcs]
            // [START vision_face_detection_gcs]
            // Specify a Google Cloud Storage uri for the image
            // or a publicly accessible HTTP or HTTPS uri.
            var image = Image.FromUri(uri);
            // [END vision_face_detection_gcs]
            // [END vision_label_detection_gcs]
            // [END vision_safe_search_detection_gcs]
            // [END vision_image_property_detection_gcs]
            // [END vision_landmark_detection_gcs]
            // [END vision_text_detection_gcs]
            // [END vision_logo_detection_gcs]
            // [END vision_web_detection_gcs]
            // [END vision_fulltext_detection_gcs]
            // [END vision_crop_hint_detection_gcs]
            return image;
        }

        static Image ImageFromFile(string filePath)
        {
            // [START vision_fulltext_detection]
            // [START vision_web_detection]
            // [START vision_crop_hint_detection]
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
            // [END vision_crop_hint_detection]
            // [END vision_web_detection]
            // [END vision_fulltext_detection]
            return image;
        }

        private static object CreateProductSet(ProductOptions opts)
        {
            Console.WriteLine("Hello! Creating a product, eh?");
            Console.WriteLine($"Project ID: {opts.ProjectID}");
            Console.WriteLine($"Location name: {opts.ComputeRegion}");
            Console.WriteLine($"Product set ID: {opts.ProductSetId}");
            Console.WriteLine($"Product display name: {opts.ProductSetDisplayName}");
            var client = ProductSearchClient.Create();

            var productSet = new ProductSet { 
                DisplayName = opts.ProductSetDisplayName
            };
            var request = new CreateProductSetRequest {
                ParentAsLocationName = new LocationName(opts.ProjectID, opts.ComputeRegion),
                ProductSetId = opts.ProductSetId,
                ProductSet = productSet
            };

            var response = client.CreateProductSet(request);
            Console.WriteLine($"Product set name: {response.DisplayName}");

            return 0;
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                ProductOptions
                >(args)
              .MapResult(
                (ProductOptions opts) => CreateProductSet(opts),
                errs => 1);

            Console.ReadKey(); // TODO(erschmid): remove before submit
        }
    }
}
