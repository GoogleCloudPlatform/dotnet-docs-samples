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

using System.Linq;
using Google.Cloud.Dlp.V2;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectStringMultipleRulesTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public InspectStringMultipleRulesTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/1159")]
        public void TestInspectStringMultipleRules_PatientRule()
        {
            string testData = "patient: Jane Doe";
            var response = InspectStringMultipleRules.Inspect(Fixture.ProjectId, testData);

            Assert.Contains(response.Result.Findings, f => f.Likelihood == Likelihood.VeryLikely);
        }

        [Fact]
        public void TestInspectStringMultipleRules_DoctorRule()
        {
            string testData = "doctor: Jane Doe";
            var response = InspectStringMultipleRules.Inspect(Fixture.ProjectId, testData);

            Assert.False(response.Result.Findings.Any());
        }

        [Fact]
        public void TestInspectStringMultipleRules_QuasimodoRule()
        {
            string testData = "patient: Quasimodo";
            var response = InspectStringMultipleRules.Inspect(Fixture.ProjectId, testData);

            Assert.False(response.Result.Findings.Any());
        }

        [Fact]
        public void TestInspectStringMultipleRules_RedactedRule()
        {
            string testData = "name of patient: REDACTED";
            var response = InspectStringMultipleRules.Inspect(Fixture.ProjectId, testData);

            Assert.False(response.Result.Findings.Any());
        }
    }
}
