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
    public class DeidentifyWithReplaceInfotypesTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;

        public DeidentifyWithReplaceInfotypesTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestDeidentifyWithReplaceInfotypes()
        {
            var testData = "My numbers are (415) 555-0890 and (415) 555-0990 and email is test@example.com.";
            var infoTypes = new InfoType[] { new InfoType { Name = "PHONE_NUMBER" }, new InfoType { Name = "EMAIL_ADDRESS" } };
            var expected = "My numbers are [PHONE_NUMBER] and [PHONE_NUMBER] and email is [EMAIL_ADDRESS].";
            var result = DeidentifyWithReplaceInfotypes.DeidentifyInfo(_fixture.ProjectId, testData, infoTypes);
            Assert.Equal(expected, result.Item.Value);
        }
    }
}