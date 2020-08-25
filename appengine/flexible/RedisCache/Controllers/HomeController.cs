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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;
using RedisCache.ViewModels;

namespace RedisCache.Controllers
{
    public class HomeController : Controller
    {
        private IDistributedCache _cache;
        private RedisCacheOptions _redisOptions;
        public HomeController(IDistributedCache cache, IOptions<RedisCacheOptions> options)
        {
            _cache = cache;
            _redisOptions = options.Value;
        }

        // [START redis_cache]
        [HttpGet]
        [HttpPost]
        public IActionResult Index(WhoForm whoForm)
        {
            if (_redisOptions == null || string.IsNullOrEmpty(_redisOptions.Configuration))
            {
                return View(new WhoCount() { MissingRedisEndpoint = true });
            }
            var model = new WhoCount()
            {
                Who = _cache.GetString("who") ?? "",
                Count = int.Parse(_cache.GetString("count") ?? "0"),
            };
            if (ModelState.IsValid && HttpContext.Request.Method.ToUpper() == "POST")
            {
                model.Who = whoForm.Who;
                model.Count += 1;
                _cache.SetString("who", model.Who ?? "");
                _cache.SetString("count", (model.Count).ToString());
            }
            return View(model);
        }
        // [END redis_cache]

        [HttpPost]
        public IActionResult Reset()
        {
            var model = new WhoCount()
            {
                Who = "",
                Count = 0,
            };
            _cache.SetString("who", "");
            _cache.SetString("count", "0");
            return View("/Views/Home/Index.cshtml", model);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
