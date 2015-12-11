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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google.Apis.Storage.v1.Data;
using Google.Apis.Storage.v1;

namespace AuthTest
{
    /// <summary>
    /// Tests the Auth code in GettingStarted.
    /// 
    /// How to run:
    /// 1. Set the environment variables:
    ///    GOOGLE_CLOUD_STORAGE_BUCKET = the Cloud Storage bucket name for which
    ///                                  to list objects.
    ///    GOOGLE_APPLICATION_CREDENTIALS = path to the .json file you 
    ///                                     downloaded from the Google 
    ///                                     Developers Console.
    /// 2. MSTest /testcontainer:test.dll   
    /// </summary>
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestListBucketContents()
        {
            var bucket = Environment.GetEnvironmentVariable(
                "GOOGLE_CLOUD_STORAGE_BUCKET"
                );
            var sample = new GoogleCloudSamples.AuthSample();
            StorageService storage = sample.CreateAuthorizedClient();
            var result = sample.ListBucketContents(storage, bucket);

            // Make sure the result is a valid list of Cloud Storage objects
            bool isStorageObjectsList = false;
            // Create a test Cloud Storage objects list instance to compare with
            // the result from the API request
            Objects validStorageObjectsList = (Objects)Activator
                .CreateInstance(typeof(Objects));
            // Compare the API request result type with the test Cloud Storage 
            // objects list to determine if the response is valid
            isStorageObjectsList =
                validStorageObjectsList.GetType() == result.GetType();
            Assert.IsTrue(isStorageObjectsList);
        }
    }
}