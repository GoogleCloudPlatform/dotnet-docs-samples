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

namespace Google.Samples
{

    /// <summary>
    /// This file contains the basic knowledge about job, including:
    ///
    /// - Construct a job with required fields
    ///
    /// - Create a job
    ///
    /// - Get a job
    ///
    /// - Update a job
    ///
    /// - Update a job with field mask
    ///
    /// - Delete a job
    /// </summary>
    public static class BasicJobSample
    {

        private static readonly JobServiceService jobService = JobServiceUtils.JobService;

        // [START basic_job]

        /// <summary>
        /// Generate a basic job with given companyName.
        /// </summary>
        public static Job GenerateJobWithRequiredFields(string companyName)
        {
            // requisition id should be a unique Id in your system.
            string requisitionId =
                "jobWithRequiredFields:" + new Random().Next();

            Job job = new Job()
            {
                RequisitionId = requisitionId,
                JobTitle = "Software Engineer",
                CompanyName = companyName,
                ApplicationUrls = { "http://careers.google.com" },
                Description =
                        "Design, develop, test, deploy, maintain and improve software.",
            };
            Console.WriteLine("Job generated: " + job);
            return job;
        }
        // [END basic_job]

        // [START create_job]

        /// <summary>
        /// Create a job.
        /// </summary>
        public static Job CreateJob(Job jobToBeCreated)
        {
            try
            {
                CreateJobRequest createJobRequest = new CreateJobRequest()
                {
                    Job = (jobToBeCreated),
                };
                Job jobCreated = jobService.Jobs.Create(createJobRequest).Execute();
                Console.WriteLine("Job created: " + jobCreated);
                return jobCreated;
            }
            catch
            {
                Console.WriteLine("Got exception while creating job");
                throw;
            }
        }
        // [END create_job]

        // [START get_job]

        /// <summary>
        /// Get a job.
        /// </summary>
        public static Job GetJob(string jobName)
        {
            try
            {
                Job jobExisted = jobService.Jobs.Get(jobName).Execute();
                Console.WriteLine("Job existed: " + jobExisted);
                return jobExisted;
            }
            catch
            {
                Console.WriteLine("Got exception while getting job");
                throw;
            }
        }
        // [END get_job]

        // [START update_job]

        /// <summary>
        /// Update a job.
        /// </summary>
        public static Job UpdateJob(string jobName, Job jobToBeUpdated)
        {
            try
            {
                UpdateJobRequest updateJobRequest = new UpdateJobRequest()
                {
                    Job = jobToBeUpdated,
                };
                Job jobUpdated = jobService.Jobs.Patch(updateJobRequest, jobName).Execute();
                Console.WriteLine("Job updated: " + jobUpdated);
                return jobUpdated;
            }
            catch
            {
                Console.WriteLine("Got exception while updating job");
                throw;
            }
        }
        //

        // [START update_job_with_field_mask]

        /**
         * Update a job.
         */
        public static Job UpdateJobWithFieldMask(string jobName, string fieldMask, Job jobToBeUpdated)
        {
            try
            {
                UpdateJobRequest updateJobRequest = new UpdateJobRequest()
                {
                    UpdateJobFields = fieldMask,
                    Job = jobToBeUpdated,
                };
                Job jobUpdated = jobService.Jobs.Patch(updateJobRequest, jobName).Execute();
                Console.WriteLine("Job updated: " + jobUpdated);
                return jobUpdated;
            }
            catch
            {
                Console.WriteLine("Got exception while updating job");
                throw;
            }
        }
        // [END update_job_with_field_mask]

        // [START delete_job]

        /**
         * Delete a job.
         */
        public static void DeleteJob(string jobName)
        {
            try
            {
                jobService.Jobs.Delete(jobName).Execute();
                Console.WriteLine("Job deleted");
            }
            catch
            {
                Console.WriteLine("Got exception while deleting job");
                throw;
            }
        }
        // [END delete_job]

        public static void Main(string[] args)
        {
            // Create a company before creating jobs
            Company companyToBeCreated = BasicCompanySample.GenerateCompany();
            Company companyCreated = BasicCompanySample.CreateCompany(companyToBeCreated);
            string companyName = companyCreated.Name;

            // Construct a job
            Job jobToBeCreated = GenerateJobWithRequiredFields(companyName);

            // Create a job
            Job jobCreated = CreateJob(jobToBeCreated);

            // Get a job
            string jobName = jobCreated.Name;
            GetJob(jobName);

            // Update a job
            Job jobToBeUpdated = jobCreated;
            jobToBeUpdated.Description = "changedDescription";
            UpdateJob(jobName, jobToBeUpdated);

            // Update a job with field mask
            UpdateJobWithFieldMask(jobName, "jobTitle", new Job() { JobTitle = ("changedJobTitle") });

            // Delete a job
            DeleteJob(jobName);

            // Delete company only after cleaning all jobs under this company
            BasicCompanySample.DeleteCompany(companyName);
        }
    }
}