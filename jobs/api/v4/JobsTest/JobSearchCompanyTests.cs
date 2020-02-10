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

using Xunit;

namespace GoogleCloudSamples
{
    public class JobSearchCompanyTests : IClassFixture<DlpTestFixture>
    {
        private readonly DlpTestFixture _fixture;
        public JobSearchCompanyTests(DlpTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestGetCompany()
        {
            ConsoleOutput output = _fixture.Run("getCompany",
                _fixture.ProjectId, _fixture.TenantId, _fixture.CompanyId);
            Assert.Contains("Company name:", output.Stdout);
        }

        [Fact]
        public void TestListCompanies()
        {
            ConsoleOutput output = _fixture.Run("listCompanies", _fixture.ProjectId, _fixture.TenantId);
            Assert.Contains("Display Name:", output.Stdout);
            Assert.Contains("External ID:", output.Stdout);
        }

        [Fact]
        public void TestCreateDeleteCompany()
        {
            ConsoleOutput output = _fixture.Run("createCompany",
                _fixture.ProjectId, _fixture.TenantId, _fixture.CompanyDisplayName, _fixture.CompanyExternalID);
            Assert.Contains("Created Company", output.Stdout);

            // Extract company id from the output.
            string[] paths = output.Stdout.Split("\n")[1].Split("/");

            string companyId = paths[paths.Length - 1];
            output = _fixture.Run("deleteCompany", _fixture.ProjectId, _fixture.TenantId, companyId);
            Assert.Contains("Deleted", output.Stdout);
        }
    }
}

