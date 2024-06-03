// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START functions_env_vars]
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace EnvironmentVariables;

public class Function : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        string foo = Environment.GetEnvironmentVariable("FOO")
            ?? "Specified environment variable is not set.";
        await context.Response.WriteAsync(foo, context.RequestAborted);
    }
}
// [END functions_env_vars]
