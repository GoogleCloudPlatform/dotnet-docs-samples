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
    public class DeidentifyDataUsingDictionaryReplacementTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public DeidentifyDataUsingDictionaryReplacementTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestDeidentify()
        {
            var infoTypes = new InfoType[] { new InfoType { Name = "PERSON_NAME" } };
            var words = new string[] { "Gary", "Justin", "David" };
            var personName = "Alicia Abernathy";
            var text = $"My name is {personName}, and my email address is aabernathy@example.com.";
            var result = DeidentifyDataUsingDictionaryReplacement.Deidentify(_fixture.ProjectId, text, infoTypes, words);
            Assert.Contains(words, result.Item.Value.Contains);
            Assert.DoesNotContain(personName, result.Item.Value);
        }
    }
}