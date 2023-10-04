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

using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.AspNetCore.DataProtection.Kms;
using Google.Cloud.AspNetCore.DataProtection.Storage;
using Microsoft.AspNetCore.DataProtection;
using System.Net;

namespace AntiForgery;

public class Program
{
    private WebApplication App {get; set;}

    // [START dotnet_data_protection]
    private Program(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddOptions();
        if (!string.IsNullOrWhiteSpace(builder.Configuration["TEST_BUCKET"]))
        {
            builder.Services.AddDataProtection()
                // Store keys in Cloud Storage so that multiple instances
                // of the web application see the same keys.
                .PersistKeysToGoogleCloudStorage(
                    builder.Configuration["TEST_BUCKET"],
                    builder.Configuration["TEST_OBJECT"])
                // Protect the keys with Google KMS for encryption and fine-
                // grained access control.
                .ProtectKeysWithGoogleKms(
                    builder.Configuration["TEST_KMS_KEY_NAME"]);
        }
        builder.Services.AddMvc();

        // App Engine sets the following 3 environment variables.
        string? projectId = Environment
            .GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");
        if (!string.IsNullOrWhiteSpace(projectId))
        {
            string service = Environment
                .GetEnvironmentVariable("GAE_SERVICE") ?? "unknown";
            string version = Environment
                .GetEnvironmentVariable("GAE_VERSION") ?? "unknown";
            builder.Services.AddGoogleDiagnosticsForAspNetCore(projectId, service, version);
        }
        App = builder.Build();
        App.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }
    // [END dotnet_data_protection]

    public static void Main(string[] args)
    {
        new Program(args).App.Run();
    }
}

