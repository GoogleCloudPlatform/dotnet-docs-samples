// Copyright 2017 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START vision_quickstart]
using System;
using System.Collections.Generic;
// Imports the Google Cloud client library
using Google.Cloud.Vision.V1;

public class QuickstartSample
{
    public static void Main()
    {
        // Instantiates a client
        ImageAnnotatorClient visionClient = ImageAnnotatorClient.Create();

        // The path to the image file to annotate
        string fileName = @"..\data\wakeupcat.jpg";

        // Load the image file into memory
        Image image = Image.FromFile(fileName);

        // Performs label detection on the image file
        IReadOnlyList<EntityAnnotation> labels = visionClient.DetectLabels(image);

        Console.WriteLine($"Labels:");
        foreach (EntityAnnotation label in labels)
        {
            Console.WriteLine($"{label.Description}");
        }
    }
}
// [END vision_quickstart]
