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
    public class DeidentifyTableWithMultipleCryptoHashTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public DeidentifyTableWithMultipleCryptoHashTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestDeidentifyTable()
        {
            var result = DeidentifyTableWithMultipleCryptoHash.Deidentify(_fixture.ProjectId);
            var serializedObject = result.ToString();

            // After de-identification the user id will not be same as the input value (user1@example.org)
            Assert.DoesNotContain("user1@example.org", serializedObject);
            // Phone number will not be de-identified so should match input value (858-555-0222)
            Assert.Contains("858-555-0222", serializedObject);
        }
    }
}
