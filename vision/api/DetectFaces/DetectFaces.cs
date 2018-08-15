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

// [START vision_face_detection_tutorial_imports]

using Google.Cloud.Vision.V1;
using System;
using System.Linq;
// [END vision_face_detection_tutorial_imports]

namespace GoogleCloudSamples
{
    public class DetectFaces
    {
        // [START vision_face_detection_tutorial_run_application]
        static readonly string s_usage = @"dotnet run image-file

        Use the Google Cloud Vision API to detect faces in the image.
        Writes an output file called image-file.faces.
        ";
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine(s_usage);
                return;
            }

            // [START vision_face_detection_tutorial_client]
            var client = ImageAnnotatorClient.Create();
            // [END vision_face_detection_tutorial_client]
            // [START vision_face_detection_tutorial_send_request]
            var response = client.DetectFaces(Image.FromFile(args[0]));
            // [END vision_face_detection_tutorial_send_request]

            int numberOfFacesFound = 0;
            // [START vision_face_detection_tutorial_process_response]
            using (var image = System.Drawing.Image.FromFile(args[0]))
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image))
            {
                var cyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 3);
                foreach (var annotation in response)
                {
                    g.DrawPolygon(cyanPen, annotation.BoundingPoly.Vertices.Select(
                        (vertex) => new System.Drawing.Point(vertex.X, vertex.Y)).ToArray());
                    // [START_EXCLUDE]
                    numberOfFacesFound++;
                    // [END_EXCLUDE]
                }
                // [START_EXCLUDE]
                int lastDot = args[0].LastIndexOf('.');
                string outFilePath = lastDot < 0 ?
                    args[0] + ".faces" :
                    args[0].Substring(0, lastDot) + ".faces" + args[0].Substring(lastDot);
                image.Save(outFilePath);
                // [END_EXCLUDE]
            }
            // [END vision_face_detection_tutorial_process_response]
            // [START_EXCLUDE]
            Console.WriteLine($"Found {numberOfFacesFound} "
                + $"face{(numberOfFacesFound == 1 ? string.Empty : "s")}.");
            // [END_EXCLUDE]
        }
        // [END vision_face_detection_tutorial_run_application]
    }
}
