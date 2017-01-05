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

// [START all]
// [START import_libraries]

using Google.Cloud.Vision.V1;
using System;
// [END import_libraries]

namespace GoogleCloudSamples
{
    public class DetectLabels
    {
        // [START run_application]
        static readonly string s_usage = @"DetectLabels image-file
        
        Use the Google Cloud Vision API to detect labels in the image.";
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine(s_usage);
                return;
            }
            Detect(args[0]);
        }
        // [END run_application]

        static void Detect(string imageFilePath)
        {
            // [START authenticate]
            var client = ImageAnnotatorClient.Create();
            // [END authenticate]
            // [START detect_labels]
            var response = client.DetectLabels(Image.FromFile(imageFilePath));
            // [END detect_labels]
            // [START parse_response]
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }
            // [END parse_response]
        }
    }
}
// [END all]
