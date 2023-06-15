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
    public class DeidentifyTableWithFpeTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public DeidentifyTableWithFpeTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestDeidentifyWithFpe()
        {
            var result = DeidentifyTableWithFpe.DeidentifyTable(_fixture.ProjectId, _fixture.KeyName, _fixture.WrappedKey);
            var serializedObject = result.ToString();
            Assert.DoesNotContain("11111", serializedObject); // 11111 is the Employee ID in the table.
            Assert.Contains("$10", serializedObject); // $10 is the compensation in the table.
        }
    }
}
