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
    public class JobSearchTenantTests : IClassFixture<DlpTestFixture>
    {
        private readonly DlpTestFixture _fixture;
        public JobSearchTenantTests(DlpTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestGetTenant()
        {
            ConsoleOutput output = _fixture.Run("getTenant",
                _fixture.ProjectId, _fixture.TenantId);
            Assert.Contains("Name:", output.Stdout);
        }

        [Fact]
        public void TestListTenants()
        {
            ConsoleOutput output = _fixture.Run("listTenants", _fixture.ProjectId);
            Assert.Contains("Tenant Name:", output.Stdout);
            Assert.Contains("External ID:", output.Stdout);
        }

        [Fact]
        public void TestCreateDeleteTenant()
        {
            // Create a tenant.
            ConsoleOutput output = _fixture.Run("createTenant", _fixture.ProjectId, _fixture.TenantExtId);
            Assert.Contains(_fixture.TenantExtId, output.Stdout);
            Assert.Contains("Created Tenant.", output.Stdout);

            // Extract the tenant id from the output.
            string[] paths = output.Stdout.Split("\n")[1].Split("/");
            string tenantId = paths[paths.Length - 1];

            // Delete tenant.
            output = _fixture.Run("deleteTenant", _fixture.ProjectId, tenantId);
            Assert.Contains("Deleted", output.Stdout);
        }
    }
}

