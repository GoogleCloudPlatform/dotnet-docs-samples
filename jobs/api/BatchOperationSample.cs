/*
 * Copyright 2018 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Google.Apis.JobService.v2;
using Google.Apis.JobService.v2.Data;
using Google.Apis.Requests;
using static Google.Apis.Requests.BatchRequest;

namespace Google.Samples
{
    /// <summary>
    /// The samples in this file introduce how to do batch operation in CJD. Including:
    ///
    /// - Create job within batch
    ///
    /// - Update job within batch
    ///
    /// - Delete job within batch.
    ///
    /// For simplicity, the samples always use the same kind of requests in each batch. In a real case ,
    /// you might put different kinds of request in one batch.
    /// </summary>
    public static class BatchOperationSample
    {
        private static JobServiceService jobService = JobServiceUtils.JobService;

        // [START batch_job_create]
        public static IList<Job> BatchCreateJobs(string companyName)
        {
            IList<Job> createdJobs = new List<Job>();
            OnResponse<Job> createCallback = (job, error, index, message) =>
                {
                    if (error != null)
                    {
                        Console.WriteLine("Create Error Message: " + error.Message);
                    }
                    else
                    {
                        Console.WriteLine("Create Job: " + job);
                        createdJobs.Add(job);
                    }
                };

            Job softwareEngineerJob = new Job
            {
                CompanyName = companyName,
                RequisitionId = "123456",
                JobTitle = "Software Engineer",
                ApplicationUrls = { "http://careers.google.com" },
                Description =
                        "Design, develop, test, deploy, maintain and improve software.",
            };
            Job hardwareEngineerJob = new Job
            {
                CompanyName = companyName,
                RequisitionId = "1234567",
                JobTitle = "Hardware Engineer",
                ApplicationUrls = { "http://careers.google.com" },
                Description =
                        "Design prototype PCBs or modify existing board designs "
                            + "to prototype new features or functions.",
            };

            // Creates batch request
            BatchRequest batchCreate = new BatchRequest(jobService);

            // Queues create job request
            var createRequest = jobService.Jobs.Create(new CreateJobRequest
            {
                Job = softwareEngineerJob,
            });
            batchCreate.Queue(createRequest, createCallback);
            var createRequest2 = jobService.Jobs.Create(new CreateJobRequest
            {
                Job = hardwareEngineerJob,
            });
            batchCreate.Queue(createRequest2, createCallback);

            // Executes batch request
            batchCreate.ExecuteAsync().RunSynchronously();
            return createdJobs;
        }
        // [END batch_job_create]

        // [START batch_job_update]
        public static IList<Job> BatchJobUpdate(IList<Job> jobsToBeUpdate)
        {
            IList<Job> updatedJobs = new List<Job>();
            OnResponse<Job> updateCallback = (job, error, index, message) =>
                {
                    if (error != null)
                    {
                        Console.WriteLine("Update Error Message: " + error.Message);
                    }
                    else
                    {
                        Console.WriteLine("Update Job: " + job);
                        updatedJobs.Add(job);
                    }
                };

            BatchRequest batchUpdate = new BatchRequest(jobService);
            // You might use Job entity with all fields filled in to do the update
            for (int i = 0; i < jobsToBeUpdate.Count; i += 2)
            {
                Job toBeUpdated = jobsToBeUpdate[i];
                toBeUpdated.JobTitle = "Engineer in Mountain View";
                var patchRequest = jobService.Jobs.Patch(new UpdateJobRequest
                {
                    Job = toBeUpdated,
                }, toBeUpdated.Name);
                batchUpdate.Queue(patchRequest, updateCallback);
            }
            // Or just fill in part of field in Job entity and set the updateJobFields
            for (int i = 1; i < jobsToBeUpdate.Count; i += 2)
            {
                Job toBeUpdated = new Job
                {
                    JobTitle = "Engineer in Mountain View",
                    Name = jobsToBeUpdate[i].Name,
                };
                var patchRequest = jobService.Jobs.Patch(
                        new UpdateJobRequest
                        {
                            Job = toBeUpdated,
                            UpdateJobFields = "jobTitle",
                        }, toBeUpdated.Name);

                batchUpdate.Queue(patchRequest, updateCallback);
            }
            batchUpdate.ExecuteAsync().RunSynchronously();

            return updatedJobs;
        }

        // [END batch_job_update]

        // [START batch_job_delete]
        public static void BatchDeleteJobs(IList<Job> jobsToBeDeleted)
        {
            BatchRequest batchDelete = new BatchRequest(jobService);
            foreach (Job job in jobsToBeDeleted)
            {
                var deleteRequest = jobService.Jobs.Delete(job.Name);
                batchDelete.Queue<Empty>(deleteRequest, (content, error, index, message) =>
                 {
                     if (error != null)
                     {
                         Console.WriteLine("Delete Error Message: " + error.Message);
                     }
                     else
                     {
                         Console.WriteLine("Job deleted");
                     }
                 });
            }
            batchDelete.ExecuteAsync().RunSynchronously();
        }

        // [END batch_job_delete]

        public static void Main(string[] args)
        {
            Company company = BasicCompanySample.CreateCompany(BasicCompanySample.GenerateCompany());

            // Batch create jobs
            IList<Job> createdJobs = BatchCreateJobs(company.Name);

            // Batch update jobs
            IList<Job> updatedJobs = BatchJobUpdate(createdJobs);

            // Batch delete jobs
            BatchDeleteJobs(updatedJobs);

            BasicCompanySample.DeleteCompany(company.Name);
        }
    }
}
