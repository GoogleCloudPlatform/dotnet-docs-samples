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

namespace test
{
    [TestClass]
    public class Test
    {
        private static string _countUniqueWordsQuery =
            "SELECT TOP(corpus, 10) as title, COUNT(*) as unique_words " +
                    "FROM [publicdata:samples.shakespeare]";

        private static string _projectId =
            System.Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        private static BigqueryService _bigquery =
            BigQuery.Util.CreateAuthorizedClient();

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
                _bigquery, new Google.Apis.Bigquery.v2.Data.QueryRequest()
            {
                Query = _countUniqueWordsQuery,
                MaxResults = 3,  // Small pages to test the paging code.
            }, _projectId).Execute();
            Assert.IsTrue(query.JobComplete ?? false);
            var rows = BigQuery.Util.GetRows(_bigquery, query.JobReference, 3);
            Assert.IsTrue(IsShakespeare(rows));
        }

        [TestMethod]
        public void TestSyncQuery10Seconds()
        {
            Assert.IsTrue(IsShakespeare(BigQuery.Util.SyncQuery(_bigquery, _projectId,
                _countUniqueWordsQuery, 10000)));
        }

        [TestMethod]
        [ExpectedException(typeof(BigQuery.Util.TimeoutException))]
        public void TestSyncQueryTimeout()
        {
            BigQuery.Util.SyncQuery(_bigquery, _projectId, _countUniqueWordsQuery, 1);
        }

        [TestMethod]
        public void TestAsyncQuery()
        {
            Job job = BigQuery.Util.AsyncQuery(_bigquery, _projectId, _countUniqueWordsQuery,
                false);
            var request = new JobsResource.GetRequest(_bigquery, _projectId,
                job.JobReference.JobId);
            job = BigQuery.Util.PollJob(request, 2000);
            var rows = BigQuery.Util.GetRows(_bigquery, job.JobReference);
            Assert.IsTrue(IsShakespeare(rows));
        }

        [TestMethod]
        public void TestListDatasets()
        {
            var datasets = BigQuery.Util.ListDatasets(_bigquery, _projectId);
            Trace.WriteLine("Datasets for " + _projectId + ":");
            foreach (var dataset in datasets)
                Trace.WriteLine(dataset.FriendlyName);
            Trace.WriteLine("");
            // TODO: Set up a project with datasets and Assert something about them here.
        }

        [TestMethod]
        public void TestListProjects()
        {
            var projects = BigQuery.Util.ListProjects(_bigquery);
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