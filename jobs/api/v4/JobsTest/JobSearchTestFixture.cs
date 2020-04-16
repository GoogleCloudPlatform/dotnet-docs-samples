// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace GoogleCloudSamples
{
    public class DlpTestFixture
    {
        public readonly string ProjectId;
        public readonly string TenantId;
        public readonly string TenantExtId;
        public readonly string CompanyId;
        public readonly string CompanyDisplayName;
        public readonly string CompanyExternalID;
        public readonly string JobId;

        readonly CommandLineRunner _commandLineRunner = new CommandLineRunner
        {
            Main = JobSearch.Main
        };

        public ConsoleOutput Run(params string[] args)
        {
            return _commandLineRunner.Run(args);
        }

        public DlpTestFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            TenantId = Environment.GetEnvironmentVariable("TEST_JOB_SEARCH_TENANT_ID");
            TenantExtId = "TENANT_EXT_ID" + TestUtil.RandomName();
            JobId = Environment.GetEnvironmentVariable("TEST_JOB_SEARCH_JOB_ID");
            CompanyId = Environment.GetEnvironmentVariable("TEST_JOB_SEARCH_COMPANY_ID");
            CompanyDisplayName = "Google Cloud DevRel";
            CompanyExternalID = $"COMPANY_EXT_ID_" + TestUtil.RandomName();
        }
    }
}
