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

using System.Collections.Generic;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectStringCustomExcludingSubstringTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public InspectStringCustomExcludingSubstringTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestInspectStringExclusionDict()
        {
            string testData = "Name: Doe, John. Name: Example, Jimmy";
            string testRegex = "[A-Z][a-z]{1,15}, [A-Z][a-z]{1,15}";
            var testList = new List<string> { "Jimmy" };
            var response = InspectStringCustomExcludingSubstring.Inspect(Fixture.ProjectId, testData, testRegex, testList);

            Assert.Contains(response.Result.Findings, f => f.Quote.Contains("Doe, John"));
            Assert.DoesNotContain(response.Result.Findings, f => f.Quote.Contains("Example, Jimmy"));
        }
    }
}
