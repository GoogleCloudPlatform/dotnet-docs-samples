// Copyright 2016 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult S(string id = null)
        {
            if (id == null)
            {
                ViewBag.Keys = Session.Keys;
                return View();
            }

            return Content((string)Session[id]);
        }

        [HttpPost]
        public ActionResult S(Models.SessionVariable svar)
        {
            Session[svar.Key] = svar.Value;
            ViewBag.Keys = Session.Keys;
            if (svar.Silent.HasValue && (bool)svar.Silent)
                return new EmptyResult();
            return View();
        }
    }
}