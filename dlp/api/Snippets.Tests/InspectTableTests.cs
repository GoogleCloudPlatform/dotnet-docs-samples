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

using Google.Cloud.Dlp.V2;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectTableTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public InspectTableTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestInspectTable()
        {
            var infoTypes = new InfoType[] { new InfoType { Name = "PHONE_NUMBER" } };
            var result = InspectTable.InspectTableData(_fixture.ProjectId, infoTypes: infoTypes);
            var serializedObject = result.ToString();
            Assert.Equal(2, result.Result.Findings.Count);
            Assert.Contains("(206) 555-0123", serializedObject); // (206) 555-0123 is the phone number in the table.
            Assert.DoesNotContain("John Doe", serializedObject); // John Doe is the person name in the table.
        }
    }
}