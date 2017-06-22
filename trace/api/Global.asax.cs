// Copyright(c) 2017 Google Inc.
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

// [START import_client_library]

using Google.Cloud.Diagnostics.AspNet;
using Google.Cloud.Diagnostics.Common;
// [END import_client_library]
using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace Trace
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        // [START configure_services_trace]
        public override void Init()
        {
            string projectId = ConfigurationManager.AppSettings["projectId"];
            // [START_EXCLUDE]
            // Confirm that projectId has been changed from placeholder value.
            if (projectId == ("YOUR-GOOGLE-PROJECT-ID"))
            {
                throw new Exception("Update Web.config and replace "
                    + "YOUR-GOOGLE-PROJECT-ID with your project id, "
                    + "and recompile.");
            }
            // [END_EXCLUDE]
            base.Init();
            TraceConfiguration traceConfig = TraceConfiguration
                .Create(bufferOptions: BufferOptions.NoBuffer());
            CloudTrace.Initialize(this, projectId, traceConfig);
        }
        // [END configure_services_trace]

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
