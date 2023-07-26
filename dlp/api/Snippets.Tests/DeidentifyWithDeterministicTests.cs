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
    public class DeidentifyWithDeterministicTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public DeidentifyWithDeterministicTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestDeidentifyWithDeterministic()
        {
            var infoTypes = new InfoType[] { new InfoType { Name = "US_SOCIAL_SECURITY_NUMBER" } };
            var surrogate = new InfoType { Name = "SSN_TOKEN" };
            var text = "My SSN is 372819127.";
            var response = DeidentifyWithDeterministic.Deidentify(_fixture.ProjectId, text, _fixture.KeyName, _fixture.WrappedKey, surrogate, infoTypes);
            Assert.NotEqual(text, response.Item.Value);
            Assert.Contains("SSN_TOKEN", response.Item.Value);
        }
    }
}
