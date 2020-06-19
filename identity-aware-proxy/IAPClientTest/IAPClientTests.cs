// Copyright (c) 2020 Google LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// https://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IAP.Samples.Tests
{
    [Collection(nameof(IAPFixture))]
    public class IAPClientTests
    {
        private readonly IAPFixture _fixture;

        public IAPClientTests(IAPFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task IAPClient_InvokeAsync()
        {
            IAPClient client = new IAPClient();
            HttpResponseMessage response = await client.InvokeRequestAsync(
                _fixture.IAPClientId, _fixture.CredentialsFilePath, _fixture.IAPUri);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
