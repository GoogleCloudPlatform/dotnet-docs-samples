// Copyright 2023 Google Inc.
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
// limitations under the License

using Google.Cloud.Dlp.V2;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectDataWithCustomRegexTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public InspectDataWithCustomRegexTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestInspectDataWithCustomRegex()
        {
            string mrn = "444-5-22222";
            string textToInspect = $"Patients MRN {mrn}";
            string customRegexPattern = "[1-9]{3}-[1-9]{1}-[1-9]{5}";
            var infoType = new InfoType { Name = "C_MRN" };
            var result = InspectDataWithCustomRegex.InspectDataCustomRegex(_fixture.ProjectId, textToInspect, customRegexPattern, infoType);
            var findings = result.Result.Findings;
            var finding = Assert.Single(findings);
            Assert.Equal(mrn, finding.Quote);
        }
    }
}