// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.Tests
{
    public class HelloWorldTest : FunctionTestBase<HelloWorld.Function>
    {
        [Fact]
        public async Task EmptyRequest()
        {
            var client = Server.CreateClient();
            var response = await client.GetAsync("uri");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello World!", responseBody);
        }
    }
}