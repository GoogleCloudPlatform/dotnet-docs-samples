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
    public class ClientLibraryQuickStartTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public ClientLibraryQuickStartTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestClientLibraryQuickStart()
        {
            var phoneNumber = "(415) 555-0890";
            var text = $"My numbers are {phoneNumber} and (415) 555-0990";
            var infoTypes = new InfoType[] { new InfoType { Name = "US_SOCIAL_SECURITY_NUMBER" }, new InfoType { Name = "PHONE_NUMBER" } };
            var result = ClientLibraryQuickStart.QuickStart(_fixture.ProjectId, text, infoTypes);
            Assert.Equal(2, result.Result.Findings.Count);
            Assert.Contains(result.Result.Findings, f => f.InfoType.Name == "PHONE_NUMBER");
            Assert.Contains(result.Result.Findings, f => f.Quote == phoneNumber);
            Assert.DoesNotContain(result.Result.Findings, f => f.InfoType.Name == "US_SOCIAL_SECURITY_NUMBER");
        }
    }
}