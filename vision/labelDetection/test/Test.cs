/*
 * Copyright (c) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of  
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google.Apis.Vision.v1;
using System.IO;
using System;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Tests the label detection code in the LabelDetectionSample.
    /// 
    /// How to run:
    /// 1. Set the environment variable:
    ///    GOOGLE_APPLICATION_CREDENTIALS = path to the .json file you 
    ///                                     downloaded from the Google 
    ///                                     Developers Console.
    /// 2. MSTest /testcontainer:test.dll   
    /// </summary>
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestDetectLabelsForValidResponse()
        {
            var sample = new GoogleCloudSamples.LabelDetectionSample();
            VisionService vision = sample.CreateAuthorizedClient();
            var result = sample.DetectLabels(vision, @"..\..\..\data\cat.jpg");
            // Confirm that DetectLabels returns expected result for test image.
            var response = result[0];
            var label = response.LabelAnnotations[0];
            Assert.IsNotNull(label.Description);
            Assert.IsTrue(label.Description.Contains("cat"));
        }

        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void TestDetectLabelsForInvalidImage()
        {
            var sample = new GoogleCloudSamples.LabelDetectionSample();
            VisionService vision = sample.CreateAuthorizedClient();
            // Confirm invalid image doesn't get labels and throws an exception 
            var result = sample.DetectLabels(vision, @"..\..\..\data\bad.txt");
            var response = result[0];
            var label = response.LabelAnnotations[0];
            if (!String.IsNullOrEmpty(label.Description))
            {
                Assert.Fail();
            }
        }
    }
}
