/*
 * Copyright (c) 2017 Google Inc.
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
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace GoogleCloudSamples.Controllers
{
    public class LoggingSampleController : Controller
    {
        // GET: LoggingSample
        public ActionResult Index()
        {
            return View();
        }

        // GET: /LoggingSample/Log/1
        [HttpGet]
        public ActionResult Log(int? count)
        {
            try
            {
                if (StackdriverLogWriter.ProjectId == "YOUR-PROJECT-ID")
                {
                    throw new Exception("Please update the YOUR-PROJECT-ID with your real project id and try again");
                }

                if (count > 0 && count <= 100)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        StackdriverLogWriter.WriteLog("Hello, Stackdriver!");
                    }
                    return Content($"Inserted {count} logs");
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Pleae limit count in range of 1 ~ 100");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
    }
}