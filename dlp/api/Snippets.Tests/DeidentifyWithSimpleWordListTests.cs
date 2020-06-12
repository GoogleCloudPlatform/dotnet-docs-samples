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
    public class DeidentifyWithSimpleWordListTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public DeidentifyWithSimpleWordListTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestDeidentifyWithSimpleWordList()
        {
            var testString = "Patient was seen in RM-YELLOW then transferred to rm green.";
            var expectedString = "Patient was seen in [CUSTOM_ROOM_ID] then transferred to [CUSTOM_ROOM_ID].";
            var result = DeidentifyWithSimpleWordList.Deidentify(Fixture.ProjectId, testString);

            Assert.Equal(expectedString, result.Item.Value);
        }
    }
}
