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
    public class DeidentifyTableUsingMaskingAndLogicTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public DeidentifyTableUsingMaskingAndLogicTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestDeidentifyMaskingAndLogic()
        {
            var result = DeidentifyTableUsingMaskingAndLogic.DeidentifyTable(_fixture.ProjectId);
            var serializedObject = result.ToString();
            Assert.Contains("*", serializedObject);
            Assert.DoesNotContain("95", serializedObject); // 95 is HAPPINESS SCORE defined in the table.
        }
    }
}