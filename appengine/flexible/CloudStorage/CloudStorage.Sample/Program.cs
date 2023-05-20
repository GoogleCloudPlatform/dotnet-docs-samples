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

using System.Net;

namespace CloudStorage;

public class Program
{
    private WebApplication App {get; set;}

    private Program(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var options = new CloudStorageOptions()
        {
            BucketName = builder.Configuration["TEST_GOOGLE_BUCKET_NAME"]
        };
        if (builder.Configuration["TEST_GOOGLE_OBJECT_NAME"] != null) {
            options.ObjectName = builder.Configuration["TEST_GOOGLE_OBJECT_NAME"];
        }
        builder.Services.AddSingleton(options);
        builder.Services.AddMvc();
        App = builder.Build();
        App.UseStaticFiles();
        App.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }

    public static void Main(string[] args)
    {
        new Program(args).App.Run();
    }
}

