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
    public class InspectStringWithoutOverlapTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public InspectStringWithoutOverlapTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestInspectStringWithoutOverlap()
        {
            string testData = "example.com is a domain, james@example.org is an email.";
            var response = InspectStringWithoutOverlap.Inspect(Fixture.ProjectId, testData);

            Assert.Contains(response.Result.Findings, f => f.Quote.Contains("example.com"));
            Assert.DoesNotContain(response.Result.Findings, f => f.Quote.Contains("example.org"));
        }
    }
}
