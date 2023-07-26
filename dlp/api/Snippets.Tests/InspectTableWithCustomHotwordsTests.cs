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

using Google.Cloud.Dlp.V2;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectTableWithCustomHotwordsTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public InspectTableWithCustomHotwordsTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestReidentifyTable()
        {
            var result = InspectTableWithCustomHotwords.InspectTable(_fixture.ProjectId);
            var serializedObject = result.ToString();
            Assert.DoesNotContain("111-11-1111", serializedObject); // 111-11-1111 is the fake social security number in the table.
            Assert.Contains("222-22-2222", serializedObject); // 222-22-2222 is the real social security number in the table.
        }
    }
}
