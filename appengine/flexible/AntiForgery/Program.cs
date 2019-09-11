// Copyright (c) 2019 Google LLC.
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

using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AntiForgery
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseDiagnostics()
                .UseStartup<Startup>();
    }

    internal static class ProgramExtensions
    {
        internal static IWebHostBuilder UseDiagnostics(
            this IWebHostBuilder builder)
        {
            // App Engine sets the following 3 environment variables.
            string projectId = Environment
                .GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");
            if (!string.IsNullOrWhiteSpace(projectId))
            {
                string service = Environment
                    .GetEnvironmentVariable("GAE_SERVICE") ?? "unknown";
                string version = Environment
                    .GetEnvironmentVariable("GAE_VERSION") ?? "unknown";
                builder.UseGoogleDiagnostics(projectId, service, version);
            }
            return builder;
        }
    }
}
