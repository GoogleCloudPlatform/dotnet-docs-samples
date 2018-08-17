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
using Google.Apis.JobService.v2;
using Google.Apis.JobService.v2.Data;

namespace Google.Samples
{

    /// <summary>
    /// This file contains the basic knowledge about company and job, including:
    ///
    /// - Construct a company with required fields
    ///
    /// - Create a company
    ///
    /// - Get a company
    ///
    /// - Update a company
    ///
    /// - Update a company with field mask
    ///
    /// - Delete a company
    /// </summary>
    public static class BasicCompanySample
    {

        private static JobServiceService jobService = JobServiceUtils.JobService;

        // [START basic_company]

        /// <summary>
        /// Generate a company
        /// </summary>
        public static Company GenerateCompany()
        {
            // distributor company id should be a unique Id in your system.
            String distributorCompanyId =
                "company:" + new Random().Next();

            Company company =
                new Company()
                {
                    DisplayName = "Google",
                    HqLocation = "1600 Amphitheatre Parkway Mountain View, CA 94043",
                    DistributorCompanyId = distributorCompanyId,
                };
            Console.WriteLine("Company generated: " + company);
            return company;
        }
        // [END basic_company]

        // [START create_company]

        /// <summary>
        /// Create a company.
        /// </summary>
        public static Company CreateCompany(Company companyToBeCreated)
        {
            try
            {
                Company companyCreated = jobService.Companies.Create(companyToBeCreated)
                    .Execute();
                Console.WriteLine("Company created: " + companyCreated);
                return companyCreated;
            }
            catch
            {
                Console.WriteLine("Got exception while creating company");
                throw;
            }
        }
        // [END create_company]

        // [START get_company]

        /// <summary>
        /// Get a company.
        /// </summary>
        public static Company GetCompany(string companyName)
        {
            try
            {
                Company companyExisted = jobService.Companies.Get(companyName).Execute();
                Console.WriteLine("Company existed: " + companyExisted);
                return companyExisted;
            }
            catch
            {
                Console.WriteLine("Got exception while getting company");
                throw;
            }
        }
        // [END get_company]

        // [START update_company]

        /// <summary>
        /// Updates a company.
        /// </summary>
        public static Company UpdateCompany(string companyName, Company companyToBeUpdated)
        {
            try
            {
                Company companyUpdated = jobService.Companies
                    .Patch(companyToBeUpdated, companyName)
                    .Execute();
                Console.WriteLine("Company updated: " + companyUpdated);
                return companyUpdated;
            }
            catch
            {
                Console.WriteLine("Got exception while updating company");
                throw;
            }
        }
        // [END update_company]

        // [START update_company_with_field_mask]

        /// <summary>
        /// Updates a company.
        /// </summary>
        public static Company UpdateCompanyWithFieldMask(string companyName, string fieldMask,
            Company companyToBeUpdated)
        {
            try
            {
                var request = jobService.Companies
                    .Patch(companyToBeUpdated, companyName);
                request.UpdateCompanyFields = fieldMask;
                Company companyUpdated = request.Execute();
                Console.WriteLine("Company updated: " + companyUpdated);
                return companyUpdated;
            }
            catch
            {
                Console.WriteLine("Got exception while updating company");
                throw;
            }
        }
        // [END update_company_with_field_mask]

        // [START delete_company]

        /// <summary>
        /// Delete a company.
        /// </summary>
        public static void DeleteCompany(string companyName)
        {
            try
            {
                jobService.Companies.Delete(companyName).Execute();
                Console.WriteLine("Company deleted");
            }
            catch
            {
                Console.WriteLine("Got exception while deleting company");
                throw;
            }
        }
        // [END delete_company]

        public static void Main(String[] args)
        {
            // Construct a company
            Company companyToBeCreated = GenerateCompany();

            // Create a company
            Company companyCreated = CreateCompany(companyToBeCreated);

            // Get a company
            String companyName = companyCreated.Name;
            GetCompany(companyName);

            // Update a company
            Company companyToBeUpdated = companyCreated;
            companyToBeUpdated.Website = "https://elgoog.im/";
            UpdateCompany(companyName, companyToBeUpdated);

            // Update a company with field mask
            UpdateCompanyWithFieldMask(companyName, "displayName",
                new Company()
                {
                    DisplayName = "changedTitle",
                    DistributorCompanyId = companyCreated.DistributorCompanyId,
                });
            // Delete a company
            DeleteCompany(companyName);
        }
    }
}
