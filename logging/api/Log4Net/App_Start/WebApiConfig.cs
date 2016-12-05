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

using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using log4net;
using System.Configuration;
using System.Xml;

namespace GoogleCloudSamples
{
    public static class WebApiConfig
    {
        // [START sample]
        /// <summary>
        /// Simple HTTP Handler that demonstrates logging using Log4Net
        /// and the Stackdriver Logging API.
        /// </summary>
        public class HelloWorldHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                string response = "";
                // Check to ensure that projectId has been changed from placeholder value.
                var section = (XmlElement)ConfigurationManager.GetSection("log4net");
                XmlElement element =
                    (XmlElement)section.GetElementsByTagName("projectId").Item(0);
                string projectId = element.Attributes["value"].Value;
                if (projectId != "YOUR-PROJECT-ID")
                {
                    // [START log4net_log_entry]
                    // Retrieve a logger for this context.
                    ILog log = LogManager.GetLogger(typeof(WebApiConfig));

                    // Log some information to Google Stackdriver Logging.
                    log.Info("Hello World.");
                    // [END log4net_log_entry]

                    // Pause for 5 seconds to ensure log entry is completed
                    Thread.Sleep(5000);
                    response = $"Log Entry created in project: {projectId}";
                }
                else
                {
                    response = "Update Web.config and replace YOUR-PROJECT" +
                    "-ID with your project id, and recompile.";
                }
                return Task.FromResult(new HttpResponseMessage()
                {
                    Content = new ByteArrayContent(Encoding.UTF8.GetBytes(response))
                });
            }
        };

        public static void Register(HttpConfiguration config)
        {
            var emptyDictionary = new HttpRouteValueDictionary();
            // Add our one HttpMessageHandler to the root path.
            config.Routes.MapHttpRoute("index", "", emptyDictionary, emptyDictionary,
                new HelloWorldHandler());
        }
        // [END sample]
    }
}
