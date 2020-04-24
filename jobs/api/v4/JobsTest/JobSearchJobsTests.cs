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
    public class JobSearchJobsTests : IClassFixture<DlpTestFixture>
    {
        private readonly DlpTestFixture _fixture;
        public JobSearchJobsTests(DlpTestFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void TestCreateDeleteJob()
        {
            //create job.
            var output = _fixture.Run("createJob",
                _fixture.ProjectId, _fixture.TenantId,
                _fixture.CompanyId, "DO_NOT_DELETE_REQ_UNIQUE_ID", "www.example.com");
            Assert.Contains("Created Job", output.Stdout);

            // Extract company id from the output.
            string[] paths = output.Stdout.Split("/");

            string jobID = paths[paths.Length - 1];

            // delete job.
            output = _fixture.Run("deleteJob", _fixture.ProjectId, _fixture.TenantId, jobID);
            Assert.Contains("Deleted Job", output.Stdout);
        }

        [Fact]
        public void TestGetJob()
        {
            var output = _fixture.Run("getJob",
                _fixture.ProjectId, _fixture.TenantId,
                _fixture.JobId);
            Assert.Contains("Job name:", output.Stdout);
            Assert.Contains("Title:", output.Stdout);
        }

        [Fact]
        public void TestListJobs()
        {
            var output = _fixture.Run("listJobs",
                _fixture.ProjectId, _fixture.TenantId,
                $"companyName=\"projects/{_fixture.ProjectId}/companies/{_fixture.CompanyId}\"");
            Assert.Contains("Job name:", output.Stdout);
            Assert.Contains("Job title:", output.Stdout);
        }

        [Fact]
        public void TestAutoCompleteJobTitle()
        {
            var output = _fixture.Run("autoCompleteJobTitle",
                _fixture.ProjectId, _fixture.TenantId, "Developer");
            Assert.Contains("Suggestion type:", output.Stdout);
            Assert.Contains("Suggested title:", output.Stdout);
        }


        [Fact]
        public void TestHistogramSearch()
        {
            var output = _fixture.Run("histogramSearchJobs",
                _fixture.ProjectId, _fixture.TenantId,
                "count(base_compensation, [bucket(12, 20)])");
            Assert.Contains("Job summary:", output.Stdout);
            Assert.Contains("Job title snippet:", output.Stdout);
            Assert.Contains("Job name:", output.Stdout);
        }

        [Fact]
        public void TestCommuteSearch()
        {
            var output = _fixture.Run("commuteSearchJobs",
                _fixture.ProjectId, _fixture.TenantId);
            Assert.Contains("Job summary:", output.Stdout);
            Assert.Contains("Job title snippet:", output.Stdout);
            Assert.Contains("Job name:", output.Stdout);
        }

        [Fact]
        public void TestCustomRankingSearch()
        {
            var output = _fixture.Run("customRankingSearchJobs",
                _fixture.ProjectId, _fixture.TenantId);
            Assert.Contains("Job summary:", output.Stdout);
            Assert.Contains("Job title snippet:", output.Stdout);
        }
    }
}

