//
// Copyright 2020 Google LLC
//
// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
//

using Google.Apis.Auth.OAuth2;
using CloudDemo.Mvc.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;

namespace CloudDemo.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private static readonly IDictionary<string, string> environmentIndicators = 
            new Dictionary<string, string>
            {
                { "KUBERNETES_SERVICE_HOST", "Kubernetes" },
                { "K_SERVICE", "Cloud Run" }
            };

        public static async Task<string> DetermineRuntime()
        {
            // Check for environment variables that indicate a specific runtime.
            foreach (var indicator in environmentIndicators)
            {
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(indicator.Key)))
                {
                    return indicator.Value;
                }
            }

            // Check metadata server, which indicates GCE.
            if (await ComputeCredential.IsRunningOnComputeEngine())
            {
                return "Compute Engine";
            }
            else
            {
                // Metadata server not available, so it's probably not nunning
                // on Google Cloud.
                return null;
            }
        }

        public async Task<ActionResult> Index()
        {
            return View(new HomeViewModel(await DetermineRuntime()));
        }
    }
}