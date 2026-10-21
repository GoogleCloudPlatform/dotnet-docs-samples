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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ContosoUniversity.DAL;

namespace ContosoUniversity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Enable legacy timestamp behavior for PostgreSQL Npgsql compatibility with DateTime
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var host = CreateHostBuilder(args).Build();

            // The .NET Generic Host (Host.CreateDefaultBuilder) automatically traps SIGTERM on Linux
            // and triggers IHostApplicationLifetime.ApplicationStopping for graceful container shutdown.
            var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            var hostLogger = host.Services.GetRequiredService<ILogger<Program>>();
            lifetime.ApplicationStopping.Register(() =>
            {
                hostLogger.LogInformation("SIGTERM / stop signal received. Application is stopping and performing graceful cleanup.");
            });

            // Ensure database is created and seeded at startup
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var context = services.GetRequiredService<SchoolContext>();
                    DbInitializer.Initialize(context);
                    logger.LogInformation("Database initialized and verified successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while initializing the database.");
                    throw;
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    // Configure ILogger to write structured JSON logs to Console (stdout) for Cloud Logging ingestion
                    logging.ClearProviders();
                    logging.AddJsonConsole(options =>
                    {
                        options.IncludeScopes = false;
                        options.TimestampFormat = "yyyy-MM-ddTHH:mm:ssZ ";
                    });
                })
                .ConfigureAppConfiguration(AddSecretConfig)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Port binding: Listen on 0.0.0.0 on the port defined by PORT env var (defaulting to 8080)
                    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
                    webBuilder.UseUrls($"http://0.0.0.0:{port}");
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
                    if (!secret.IsDirectory && secret.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                        config.AddJsonFile(secret.PhysicalPath, false, true);
                }
            }
        }
    }
}
