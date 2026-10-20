/**
 * Copyright 2025 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License
 */

// This application entry point is based on ASP.NET Core new project templates and is included
// as a starting point for app host configuration.
// This file may need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core hosting, see https://docs.microsoft.com/aspnet/core/fundamentals/host/web-host

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Google.Cloud.Diagnostics.AspNetCore;

namespace ContosoUniversity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(AddSecretConfig)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (webBuilder.GetSetting("ENVIRONMENT") == "Production")
                    {
                        webBuilder.UseGoogleDiagnostics();
                    }
                    webBuilder.UseStartup<Startup>();
                });

        private static void AddSecretConfig(HostBuilderContext context,
            IConfigurationBuilder config)
        {
            string secretsPath = context.Configuration.GetValue("SECRETS_PATH",
                "secrets");

            var secretFileProvider = context.HostingEnvironment.ContentRootFileProvider
                .GetDirectoryContents(secretsPath);

            if (secretFileProvider.Exists)
            {
                foreach (var secret in secretFileProvider)
                {
                    if (!secret.IsDirectory && secret.Name.ToUpper().EndsWith(".JSON"))
                        config.AddJsonFile(secret.PhysicalPath, false, true);
                }
            }
        }
    }
}
