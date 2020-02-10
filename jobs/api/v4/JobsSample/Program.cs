/*
 * Copyright (c) 2020 Google Inc.
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
using CommandLine;
using Google.Cloud.Talent.V4Beta1;

namespace GoogleCloudSamples
{
    internal abstract class BaseOptions
    {
        [Value(0, HelpText = "Your Google Cloud Project ID.", Required = true)]
        public string ProjectId { get; set; }
    }

    [Verb("createTenant", HelpText = "Create Tenant for scoping resources, e.g. companies and jobs.")]
    internal class CreateTenantOptions : BaseOptions
    {
        [Value(1, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string TenantExternalId { get; set; }
    }

    [Verb("createCompany", HelpText = "Create Tenant for scoping resources, e.g. companies and jobs.")]
    internal class CreateCompanyOptions : BaseOptions
    {
        [Value(1, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string TenantId { get; set; }
        [Value(2, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string DisplayName { get; set; }
        [Value(3, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string CompanyExternalId { get; set; }
    }

    [Verb("getTenant", HelpText = "Create Tenant for scoping resources, e.g. companies and jobs.")]
    internal class GetTenantOptions : BaseOptions
    {
        [Value(1, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string TenantId { get; set; }
    }


    [Verb("deleteTenant", HelpText = "Create Tenant for scoping resources, e.g. companies and jobs.")]
    internal class DeleteTenantOptions : BaseOptions
    {
        [Value(1, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string TenantId { get; set; }
    }

    [Verb("deleteCompany", HelpText = "Create Tenant for scoping resources, e.g. companies and jobs.")]
    internal class DeleteCompanyOptions : BaseOptions
    {
        [Value(1, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string TenantId { get; set; }
        [Value(2, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string CompanyId { get; set; }
    }

    [Verb("listTenants", HelpText = "Create Tenant for scoping resources, e.g. companies and jobs.")]
    internal class ListTenantsOptions : BaseOptions
    {
    }

    [Verb("getCompany", HelpText = "Create Tenant for scoping resources, e.g. companies and jobs.")]
    internal class GetCommpanyOptions : BaseOptions
    {
        [Value(1, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string TenantId { get; set; }

        [Value(2, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string CompanyId { get; set; }
    }

    [Verb("listCompanies", HelpText = "Create Tenant for scoping resources, e.g. companies and jobs.")]
    internal class ListCompaniesOptions : BaseOptions
    {
        [Value(1, HelpText = "Your Unique Identifier for Tenant.", Required = true)]
        public string TenantId { get; set; }
    }

    public class JobSearch
    {
        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            verbMap
                .Add((ListTenantsOptions opts) => ListTenantsSample.ListTenants(
                    opts.ProjectId))
                .Add((CreateCompanyOptions opts) => CreateCompanySample.CreateCompany(opts.ProjectId, opts.TenantId,
                        opts.DisplayName, opts.CompanyExternalId))
                .Add((GetCommpanyOptions opts) => GetCompanySample.GetCompany(opts.ProjectId, opts.TenantId, opts.CompanyId))
                .Add((ListCompaniesOptions opts) => ListCompaniesSample.ListCompanies(opts.ProjectId, opts.TenantId))
                .Add((CreateTenantOptions opts) => CreateTenantSample.CreateTenant(opts.ProjectId, opts.TenantExternalId))
                .Add((GetTenantOptions opts) => GetTenantSample.GetTenant(opts.ProjectId, opts.TenantId))
                .Add((DeleteTenantOptions opts) => DeleteTenantSample.DeleteTenant(opts.ProjectId, opts.TenantId))
                .Add((DeleteCompanyOptions opts) => DeleteCompanySample.DeleteCompany(opts.ProjectId, opts.TenantId, opts.CompanyId))
                .NotParsedFunc = (err) => 1;
            return (int)verbMap.Run(args);
        }
    }
}
