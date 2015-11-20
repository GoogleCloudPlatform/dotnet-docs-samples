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

using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;

namespace GoogleCloudSamples
{
    [TestClass]
    public class Test
    {
        private static readonly string s_countUniqueWordsQuery =
            "SELECT TOP(corpus, 10) as title, COUNT(*) as unique_words " +
                    "FROM [publicdata:samples.shakespeare]";

        private static readonly string s_projectId =
            System.Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        private static readonly BigqueryUtil s_util = new BigqueryUtil();
        private static readonly BigqueryService s_bigquery =
            s_util.CreateAuthorizedClient();

        private static bool IsShakespeare(IEnumerable<TableRow> rows)
        {
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
            return foundHamlet && foundKingLear;
        }

        [TestMethod]
        public void TestGetRows()
        {
            var query = new Google.Apis.Bigquery.v2.JobsResource.QueryRequest(
                s_bigquery, new Google.Apis.Bigquery.v2.Data.QueryRequest()
                {
                    Query = s_countUniqueWordsQuery,
                    MaxResults = 3,  // Small pages to test the paging code.
                }, s_projectId).Execute();
            Assert.IsTrue(query.JobComplete ?? false);
            var rows = s_util.GetRows(query.JobReference, 3);
            Assert.IsTrue(IsShakespeare(rows));
        }

        [TestMethod]
        public void TestSyncQuery10Seconds()
        {
            Assert.IsTrue(IsShakespeare(s_util.SyncQuery(s_projectId,
                s_countUniqueWordsQuery, 10000)));
        }

        [TestMethod]
        [ExpectedException(typeof(BigqueryUtil.TimeoutException))]
        public void TestSyncQueryTimeout()
        {
            s_util.SyncQuery(s_projectId, s_countUniqueWordsQuery, 1);
        }

        [TestMethod]
        public void TestAsyncQuery()
        {
            Job job = s_util.AsyncQuery(s_projectId,
                s_countUniqueWordsQuery, false);
            var request = new JobsResource.GetRequest(s_bigquery, s_projectId,
                job.JobReference.JobId);
            job = s_util.PollJob(request, 2000);
            var rows = s_util.GetRows(job.JobReference);
            Assert.IsTrue(IsShakespeare(rows));
        }

        [TestMethod]
        public void TestListDatasets()
        {
            var datasets = s_util.ListDatasets(s_projectId);
            Trace.WriteLine("Datasets for " + s_projectId + ":");
            foreach (var dataset in datasets)
                Trace.WriteLine(dataset.FriendlyName);
            Trace.WriteLine("");
        }

        [TestMethod]
        public void TestListProjects()
        {
            var projects = s_util.ListProjects();
            Trace.WriteLine("Projects:");
            int count = 0;
            foreach (var project in projects)
            {
                Trace.WriteLine(project.FriendlyName);
                ++count;
            }
            Trace.WriteLine("");
            Assert.IsTrue(count > 0);
        }
    }
}