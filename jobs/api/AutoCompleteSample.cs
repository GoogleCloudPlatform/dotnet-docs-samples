// TODO(acasale): Completion API doesn't appear to be available in the C# lib.

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
using System.Threading;
using Google.Apis.JobService.v2;
using Google.Apis.JobService.v2.Data;

namespace Google.Samples
{

    /// <summary>
    /// The samples in this file introduced how to do the auto complete, including:
    ///
    /// - Default auto complete (on both company display name and job title)
    ///
    /// - Auto complete on job title only
    /// </summary>
    public static class AutoCompleteSample
    {

        private static readonly JobServiceService jobService = JobServiceUtils.JobService;

        //[START auto_complete_job_title]

        /// <summary>
        /// Auto completes job titles within given companyName.
        /// </summary>
        public static void JobTitleAutoComplete(string companyName, string query)
        {
            var complete = jobService.V2.Complete();
            complete.Query = query;
            complete.LanguageCode = "en-US";
            complete.Type = V2Resource.CompleteRequest.TypeEnum.JOBTITLE;
            complete.PageSize = 10;
            if (companyName != null)
            {
                complete.CompanyName = companyName;
            }

            CompleteQueryResponse results = complete.Execute();

            Console.WriteLine(results);
        }
        // [END auto_complete_default]

        /// <summary>
        /// Auto completes job titles within given companyName.
        /// </summary>
        public static void DefaultAutoComplete(string companyName, string query)
        {
            var complete = jobService.V2.Complete();
            complete.Query = query;
            complete.LanguageCode = "en-US";
            complete.PageSize = 10;
            if (companyName != null)
            {
                complete.CompanyName = companyName;
            }

            CompleteQueryResponse results = complete.Execute();

            Console.WriteLine(results);
        }
        // [END auto_complete_default]

        public static void Main(string[] args)
        {
            Company companyToBeCreated = BasicCompanySample.GenerateCompany();
            companyToBeCreated.DisplayName = "Google";
            string companyName = BasicCompanySample.CreateCompany(companyToBeCreated).Name;

            Job jobToBeCreated = BasicJobSample.GenerateJobWithRequiredFields(companyName);
            jobToBeCreated.JobTitle = "Software engineer";
            string jobName = BasicJobSample.CreateJob(jobToBeCreated).Name;

            // Wait several seconds for post processing
            Thread.Sleep(10000);
            DefaultAutoComplete(companyName, "goo");
            DefaultAutoComplete(companyName, "sof");
            JobTitleAutoComplete(companyName, "sof");

            BasicJobSample.DeleteJob(jobName);
            BasicCompanySample.DeleteCompany(companyName);
        }
    }
}