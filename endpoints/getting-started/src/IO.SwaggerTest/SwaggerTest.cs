// Copyright (c) 2018 Google LLC.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace IO.SwaggerTest
{
    public class SwaggerTest
    {
        class Payload
        {
            public string Message { get; set; }
        }

        [Fact]
        public async void TestEcho()
        {
            const string MESSAGE = "in a bottle";
            var http = new HttpClient();
            var payload = new Payload { Message = MESSAGE };
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(payload)));
            content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            string uri = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
                + "/echo";
            var response = await http.PostAsync(uri, content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var responsePayload = JsonConvert
                .DeserializeObject<Payload>(responseString);
            Assert.Equal(MESSAGE, payload.Message);
        }
    }
}
