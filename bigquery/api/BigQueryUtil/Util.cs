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

using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GoogleCloudSamples
{
    public class BigqueryUtil
    {
        public class BigQueryException : Exception { }

        public class TimeoutException : BigQueryException { }

        /// <summary>
        /// Creates an authorized Bigquery client service using Application
        /// Default Credentials.
        /// </summary>
        /// <returns>an authorized Bigquery client</returns>
        // [START build_service]
        public BigqueryService CreateAuthorizedClient()
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Bigquery scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    BigqueryService.Scope.Bigquery
                });
            }
            return new BigqueryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DotNet Bigquery Samples",
            });
        }
        // [END build_service]

        /// <summary>
        /// Pages through the results of an arbitrary Bigquery request.
        /// </summary>
        /// <param name="jobRef">The job whose rows will be fetched.</param>
        /// <param name="rowsPerPage">How many rows to fetch in each http 
        /// request?</param>
        /// <returns>An IEnumerable of rows.</returns>
        // [START paging]
        public IEnumerable<TableRow> GetRows(JobReference jobRef,
            long? rowsPerPage = null)
        {
            BigqueryService bigquery = CreateAuthorizedClient();
            var request = new JobsResource.GetQueryResultsRequest(
                bigquery, jobRef.ProjectId, jobRef.JobId);
            request.MaxResults = rowsPerPage;
            do
            {
                var response = request.Execute();
                if (response.Rows != null)
                    foreach (var row in response.Rows)
                        yield return row;
                request.PageToken = response.PageToken;
            } while (!String.IsNullOrEmpty(request.PageToken));
        }
        // [END paging]

        /// <summary>Perform the given query using the synchronous api.
        /// </summary>
        /// <param name="projectId">project id from developer console</param>
        /// <param name="queryString">query to run</param>
        /// <param name="timeoutMs">Timeout in milliseconds before we abort
        /// </param>
        /// <returns>The results of the query</returns>
        /// <exception cref="TimeoutException">If the request times out.
        /// </exception>
        // [START sync_query]
        public IEnumerable<TableRow> SyncQuery(string projectId,
            string queryString, long timeoutMs)
        {
            BigqueryService bigquery = CreateAuthorizedClient();
            var query = new JobsResource.QueryRequest(
                bigquery, new QueryRequest()
                {
                    Query = queryString,
                    TimeoutMs = timeoutMs,
                }, projectId).Execute();
            if (query.JobComplete != true)
                throw new TimeoutException();
            return query.Rows ?? new TableRow[] { };
        }
        // [END sync_query]

        /// <summary>Inserts an asynchronous query Job for a particular query.
        /// </summary>
        /// </param>
        /// <param name="projectId">a string containing the project ID</param>
        /// <param name="querySql">the actual query string</param>
        /// <param name="batch">true if you want to run the query as BATCH
        /// </param>
        /// <returns>a reference to the inserted query job</returns>
        // [START async_query]
        public Job AsyncQuery(string projectId, string querySql, bool batch)
        {
            BigqueryService bigquery = CreateAuthorizedClient();
            JobConfigurationQuery queryConfig = new JobConfigurationQuery
            {
                Query = querySql,
                Priority = batch ? "BATCH" : null
            };
            JobConfiguration config =
                new JobConfiguration { Query = queryConfig };
            Job job = new Job { Configuration = config };
            return bigquery.Jobs.Insert(job, projectId).Execute();
        }
        // [END async_query]

        /// <summary>
        /// Polls the job for completion.
        /// </summary>
        /// <param name="request">The bigquery request to poll for completion
        /// </param>
        /// <param name="interval">Number of milliseconds between each poll
        /// </param>
        /// <returns>The finished job</returns>
        // [START poll_job]
        public Job PollJob(JobsResource.GetRequest request, int interval)
        {
            Job job = request.Execute();
            while (job.Status.State != "DONE")
            {
                Thread.Sleep(interval);
                job = request.Execute();
            }
            return job;
        }
        // [END poll_job]

        /// <summary>
        /// Lists all Datasets in a project specified by the projectId.
        /// </summary>
        /// <param name="projectId">The projectId from which lists the existing
        /// Datasets.</param>
        // [START list_datasets]
        public IEnumerable<DatasetList.DatasetsData> ListDatasets(
            string projectId)
        {
            BigqueryService bigquery = CreateAuthorizedClient();
            var datasetRequest =
                new DatasetsResource.ListRequest(bigquery, projectId);
            // Sometimes Datasets will be null instead of an empty list.
            // It's easy to forget that and dereference null.  So, catch
            // that case and return an empty list.
            return datasetRequest.Execute().Datasets ??
                new DatasetList.DatasetsData[] { };
        }
        // [END list_datasets]

        /// <summary>
        /// Lists all Projects.
        /// </summary>
        // [START list_projects]
        public IEnumerable<ProjectList.ProjectsData> ListProjects()
        {
            BigqueryService bigquery = CreateAuthorizedClient();
            var projectRequest = new ProjectsResource.ListRequest(bigquery);
            // Sometimes Projects will be null instead of an empty list.
            // It's easy to forget that and dereference null.  So, catch
            // that case and return an empty list.
            return projectRequest.Execute().Projects ??
                new ProjectList.ProjectsData[] { };
        }
        // [END list_projects]
    }
}