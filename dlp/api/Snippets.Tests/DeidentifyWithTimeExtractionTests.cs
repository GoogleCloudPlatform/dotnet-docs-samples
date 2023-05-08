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
// limitations under the License.

using Xunit;

namespace GoogleCloudSamples
{
    public class DeidentifyWithTimeExtractionTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public DeidentifyWithTimeExtractionTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestDeidentifyWithTimeExtraction()
        {
            var result = DeidentifyWithTimeExtraction.Deidentify(_fixture.ProjectId);
            var serializedObject = result.ToString();
            Assert.Contains("2022", serializedObject); // 2022 is the year mentioned in the table
            Assert.DoesNotContain("10", serializedObject); // 10 is the day mentioned in the table
        }
    }
}
