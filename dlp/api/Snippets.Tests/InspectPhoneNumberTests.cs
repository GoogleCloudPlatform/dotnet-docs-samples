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

using Xunit;

namespace GoogleCloudSamples
{
    public class InspectPhoneNumbersTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public InspectPhoneNumbersTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestInspectPhoneNumber()
        {
            var phoneNumber = "(415) 555-0890";
            var ssn = "372819127";
            var text = $"My numbers are {phoneNumber} and (415) 555-0990; and SSN is {ssn}.";
            var result = InspectPhoneNumber.Inspect(_fixture.ProjectId, text);
            Assert.Equal(2, result.Result.Findings.Count);
            Assert.Contains(result.Result.Findings, f => f.InfoType.Name == "PHONE_NUMBER" && f.Quote == phoneNumber);
            Assert.DoesNotContain(result.Result.Findings, f => f.Quote == ssn);
        }
    }
}