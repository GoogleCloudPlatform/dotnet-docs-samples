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
            request.ContentType= "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(
                    new Payload { Message = "in a bottle" }));
            }
            var response = (HttpWebResponse) await request.GetResponseAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseString = await new StreamReader(
                response.GetResponseStream()).ReadToEndAsync();
            var payload = JsonConvert.DeserializeObject<Payload>(responseString);       
            Assert.Equal("in a bottle", payload.Message);
        }
    }
}
