// Copyright(c) 2016 Google Inc.
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
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.ExceptionHandling;
using Google.Cloud.Diagnostics.AspNet;

namespace GoogleCloudSamples
{
    public static class WebApiConfig
    {
        // [START sample]
        /// <summary>
        /// Simple HTTP Handler that demonstrates error reporting 
        /// via the Stackdriver Error Reporting API.
        /// </summary>
        public class HelloWorldHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                // Simulate an exception.
                bool exception = true;
                if (exception)
                {
                    throw new Exception("Generic exception for testing Stackdriver Error Reporting");
                }
                return Task.FromResult(new HttpResponseMessage()
                {
                    Content = new ByteArrayContent(Encoding.UTF8.GetBytes("Hello World."))
                });
            }
        };

        // [START register_error_reporting]
        public static void Register(HttpConfiguration config)
        {
            string projectId = "YOUR-PROJECT-ID";
            string serviceName = "NAME-OF-YOUR-SERVICE";
            string version = "VERSION-OF-YOUR-SERVCICE";
            // [START _EXCLUDE]
            // Check to ensure that projectId has been changed from placeholder value.
            if (projectId == ("YOUR-PROJECT" + "-ID"))
            {
                throw new Exception("Set string projectId above to be your project id.");
            }
            // [END _EXCLUDE]
            // Add a catch all for the uncaught exceptions.
            config.Services.Add(typeof(IExceptionLogger),
                ErrorReportingExceptionLogger.Create(projectId, serviceName, version));
            // [START _EXCLUDE]
            var emptyDictionary = new HttpRouteValueDictionary();
            // Add our one HttpMessageHandler to the root path.
            config.Routes.MapHttpRoute("index", "", emptyDictionary, emptyDictionary,
                new HelloWorldHandler());
            // [END _EXCLUDE]
        }
        // [END register_error_reporting]
        // [END sample]
    }
}
