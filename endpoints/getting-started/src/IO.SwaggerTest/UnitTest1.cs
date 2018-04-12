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

namespace IO.SwaggerTest
{
    public class UnitTest1
    {
        class Payload
        {
            public string Message { get; set; }
        }

        [Fact]
        public async void TestEcho()
        {
            var request = HttpWebRequest.Create("http://localhost:7412/echo");
            request.Method = "POST";
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(
                    new Payload { Message = "in a bottle" }));
            }
            var response = (HttpWebResponse)await request.GetResponseAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseString = await new StreamReader(
                response.GetResponseStream()).ReadToEndAsync();
            var payload = JsonConvert.DeserializeObject<Payload>(responseString);
            Assert.Equal("in a bottle", payload.Message);
        }
    }
}
