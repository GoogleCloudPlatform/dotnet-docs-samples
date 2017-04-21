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

using Microsoft.AspNetCore.Mvc;
using Stackdriver.ViewModels;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Stackdriver.Controllers
{
    public class HomeController : Controller
    {
        readonly string _projectId;
        public HomeController(IOptions<StackdriverOptions> options)
        {
            _projectId = options.Value.ProjectId;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new HomeIndex() { ProjectId = _projectId };
            return View(model);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
