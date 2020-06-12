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
    public class InspectStringWithExclusionDictTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public InspectStringWithExclusionDictTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestInspectStringExclusionDict()
        {
            string testData = "Some email addresses: gary@example.com, example@example.com";
            var response = InspectStringWithExclusionDict.Inspect(Fixture.ProjectId, testData, new List<string> { "example@example.com" });

            Assert.Contains(response.Result.Findings, f => f.Quote.Contains("gary@example.com"));
            Assert.DoesNotContain(response.Result.Findings, f => f.Quote.Contains("example@example.com"));
        }
    }
}
