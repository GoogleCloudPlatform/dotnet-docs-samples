﻿// Copyright 2020 Google LLC
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

// [START functions_log_helloworld]
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LogHelloWorld;

public class Function : IHttpFunction
{
    private readonly ILogger _logger;

    public Function(ILogger<Function> logger) =>
        _logger = logger;

    public async Task HandleAsync(HttpContext context)
    {
        Console.WriteLine("I am a log to stdout!");
        Console.Error.WriteLine("I am a log to stderr!");

        _logger.LogInformation("I am an info log!");
        _logger.LogWarning("I am a warning log!");

        await context.Response.WriteAsync("Messages successfully logged!");
    }
}
// [END functions_log_helloworld]
