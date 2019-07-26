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

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace TranslateWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().UsePortEnvironmentVariable();
    }

    static class ProgramExtensions
    {
        // Google Cloud Run sets the PORT environment variable to tell this
        // process which port to listen to.
        public static IWebHostBuilder UsePortEnvironmentVariable(
            this IWebHostBuilder builder)
        {
            string port = Environment.GetEnvironmentVariable("PORT");
            if (!string.IsNullOrEmpty(port))
            {
                builder.UseUrls($"http://0.0.0.0:{port}");
            }
            return builder;
        }
    }
}
