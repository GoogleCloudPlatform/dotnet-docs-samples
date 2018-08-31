/*
 * Copyright (c) 2018 Google Inc.
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
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudTalentSolution.v3;
using Google.Apis.CloudTalentSolution.v3.Data;
using Google.Apis.Services;
using CommandLine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GoogleCloudSamples
{
    public class Program
    {
        public static GoogleCredential credential;
        public static CloudTalentSolutionService jobServiceClient;
        public static string projectId;
        public static string parent;
        public static Company myCompany;

        [Verb("listCompanies", HelpText = "List all companies.")]
        class ListCompaniesOptions
        {
        }

        [Verb("createCompany", HelpText = "Create a company.")]
        class CreateCompanyOptions
        {
            [Value(0, HelpText = "Company to be created in seralized JSON.", Required = true)]
            public string companyJson { get; set; }
        }

        [Verb("deleteCompany", HelpText = "Delete a company.")]
        class DeleteCompanyOptions
        {
            [Value(0, HelpText = "Name of the company to be deleted.", Required = true)]
            public string companyName { get; set; }
        }

        [Verb("createJob", HelpText = "Create a job.")]
        class CreateJobOptions
        {
            [Value(0, HelpText = "Job to be created in seralized JSON.", Required = true)]
            public string jobJson { get; set; }
        }

        [Verb("updateJob", HelpText = "Update a job.")]
        class UpdateJobOptions
        {
            [Value(0, HelpText = "Name of job to be updated.", Required = true)]
            public string jobName { get; set; }
            [Value(1, HelpText = "Updated job in seralized JSON.", Required = true)]
            public string jobJson { get; set; }
        }

        [Verb("deleteJob", HelpText = "Delete a job.")]
        class DeleteJobOptions
        {
            [Value(0, HelpText = "Name of job to be deleted.", Required = true)]
            public string jobName { get; set; }
        }

        [Verb("searchJobs", HelpText = "Search job listings for keyword.")]
        class SearchJobsOptions
        {
            [Value(0, HelpText = "Name of company to be searched.", Required = true)]
            public string companyName { get; set; }
            [Value(1, HelpText = "Search query.", Required = true)]
            public string searchQuery { get; set; }
        }

        public static object ListCompanies()
        {
            QuickStart.Main(null);
            return 0;
        }

        public static object CreateCompanyJson(string companyJson)
        {
            Company company = JsonConvert.DeserializeObject<Company>(companyJson);
            Companies.Initialize();
            Companies.CreateCompany(company);
            return 0;
        }

        public static object DeleteCompany(string companyName)
        {
            QuickStartCompanies.Initialize();
            QuickStartCompanies.DeleteCompany(companyName);
            return 0;
        }

        public static object CreateJobJson(string jobJson)
        {
            Job job = JsonConvert.DeserializeObject<Job>(jobJson);
            Jobs.Initialize();
            Jobs.CreateJob(job);
            return 0;
        }

        public static object UpdateJob(string jobName, string jobJson)
        {
            Job job = JsonConvert.DeserializeObject<Job>(jobJson);
            Jobs.Initialize();
            Jobs.UpdateJob(jobName, job);
            return 0;
        }
        public static object DeleteJob(string jobName)
        {
            Jobs.Initialize();
            Jobs.DeleteJob(jobName);
            return 0;
        }

        public static object SearchJobs(string companyName, string searchQuery)
        {
            QuickStartSearch.Initialize();
            QuickStartSearch.BasicSearchJobs(companyName, searchQuery);
            return 0;
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                CreateCompanyOptions, ListCompaniesOptions, DeleteCompanyOptions,
                CreateJobOptions, UpdateJobOptions, DeleteJobOptions, SearchJobsOptions
                >(args)
              .MapResult(
                (ListCompaniesOptions opts) => ListCompanies(),
                (CreateCompanyOptions opts) => CreateCompanyJson(opts.companyJson),
                (DeleteCompanyOptions opts) => DeleteCompany(opts.companyName),
                (CreateJobOptions opts) => CreateJobJson(opts.jobJson),
                (UpdateJobOptions opts) => UpdateJob(opts.jobName, opts.jobJson),
                (DeleteJobOptions opts) => DeleteJob(opts.jobName),
                (SearchJobsOptions opts) => SearchJobs(opts.companyName, opts.searchQuery),
                errs => 1);
        }
    }
}
