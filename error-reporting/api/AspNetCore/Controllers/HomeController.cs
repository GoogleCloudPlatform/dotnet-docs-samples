/*
 * Copyright (c) 2018 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using ErrorReporting.Models;

namespace ErrorReporting.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            // Generate an error that Stackdriver should capture
            bool exception = true;
            if (exception)
            {
                // [START error_reporting_setup_dotnetcore_force_error_controller]
                throw new Exception("Generic exception for testing Stackdriver Error Reporting");
                // [END error_reporting_setup_dotnetcore_force_error_controller]
            }
            return new HttpResponseMessage
            {
                Content = new ByteArrayContent(Encoding.UTF8.GetBytes("Hello World."))
            };
        }
    }
}
