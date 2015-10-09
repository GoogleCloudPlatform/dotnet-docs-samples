﻿/*
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
using Google.Apis.Bigquery.v2.Data;

namespace BQTest
{
    /// <summary>
    /// Tests the BigQuery code in GettingStarted.
    /// 
    /// How to run:
    /// 1. Set the environment variables:
    ///    GOOGLE_PROJECT_ID = your project id displayed on the Google Developers Console.
    ///    GOOGLE_APPLICATION_CREDENTIALS = path to the .json file you downloaded from the
    ///      Google Developers Console.
    /// 2. MSTest /testcontainer:BigQueryTest.dll   
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestShakespeare()
        {
            var bigquery = BQSample.Program.CreateAuthorizedClientAsync().Result;
            var rows = BQSample.Program.ExecuteQueryAsync(
                "SELECT TOP(corpus, 10) as title, COUNT(*) as unique_words " +
                "FROM [publicdata:samples.shakespeare]", bigquery,
                System.Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID")).Result;

            // Make sure the results look like shakespeare.
            bool foundKingLear = false;
            bool foundHamlet = false;
            foreach (TableRow row in rows)
            {
                foreach (TableCell field in row.F)
                {
                    foundKingLear = foundKingLear || "kinglear".Equals(field.V);
                    foundHamlet = foundHamlet || "hamlet".Equals(field.V);
                }
            }
            Assert.IsTrue(foundHamlet);
            Assert.IsTrue(foundKingLear);
        }
    }
}
