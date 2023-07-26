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
using Google.Cloud.Dlp.V2;

namespace GoogleCloudSamples
{
    public class ReidenitifyContentUsingDeterministicTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public ReidenitifyContentUsingDeterministicTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestReidentifyWithDeterministic()
        {
            var infoTypes = new InfoType[] { new InfoType { Name = "PHONE_NUMBER" } };
            var surrogate = new InfoType { Name = "PHONE_TOKEN" };
            var inputText = "My phone number is 1234567890.";
            var response = DeidentifyWithDeterministic.Deidentify(_fixture.ProjectId, inputText, _fixture.KeyName, _fixture.WrappedKey, surrogate, infoTypes);
            var reidentifyInput = response.Item.Value;
            var result = ReidentifyContentUsingDeterministic.ReidentifyDeterministic(_fixture.ProjectId, reidentifyInput, _fixture.KeyName, _fixture.WrappedKey, surrogate);
            Assert.Equal(inputText, result.Item.Value);
        }
    }
}
