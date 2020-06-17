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
    public class InspectStringOmitOverlapTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public InspectStringOmitOverlapTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestInspectStringOmitOverlapTests()
        {
            string testData = "james@example.com";
            var response = InspectStringOmitOverlap.Inspect(Fixture.ProjectId, testData);

            Assert.Contains(response.Result.Findings, f => f.InfoType.Name == "EMAIL_ADDRESS");
            Assert.DoesNotContain(response.Result.Findings, f => f.InfoType.Name == "PERSON_NAME");
        }
    }
}