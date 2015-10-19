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

namespace test
{
    /// <summary>
    /// Tests the cloudstorage sample.
    /// 
    /// How to run:
    /// 1. Set the environment variables:
    ///    GOOGLE_PROJECT_ID = your project id displayed on the Google Developers Console.
    ///    GOOGLE_APPLICATION_CREDENTIALS = path to the .json file you downloaded from the
    ///      Google Developers Console.
    ///    GOOGLE_BUCKET = the name of your bucket in Google Cloud Storage.
    /// 2. MSTest /testcontainer:test.dll   
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestRun()
        {
            // Just observe that it doesn't throw an exception.
            new GCSSearch.Program().Run(System.Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"),
                System.Environment.GetEnvironmentVariable("GOOGLE_BUCKET")).Wait();
        }
    }
}
