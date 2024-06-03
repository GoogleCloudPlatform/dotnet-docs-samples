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

// TODO: Only try to parse the JSON if the content type is application/json?
// TODO: Fail the request if the JSON is invalid?

// [START functions_helloworld_http]
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace HelloHttp;

public class Function : IHttpFunction
{
    private readonly ILogger _logger;

    public Function(ILogger<Function> logger) =>
        _logger = logger;

    public async Task HandleAsync(HttpContext context)
    {
        HttpRequest request = context.Request;
        // Check URL parameters for "name" field
        // "world" is the default value
        string name = ((string) request.Query["name"]) ?? "world";

        // If there's a body, parse it as JSON and check for "name" field.
        using TextReader reader = new StreamReader(request.Body);
        string text = await reader.ReadToEndAsync();
        if (text.Length > 0)
        {
            try
            {
                JsonElement json = JsonSerializer.Deserialize<JsonElement>(text);
                if (json.TryGetProperty("name", out JsonElement nameElement) &&
                    nameElement.ValueKind == JsonValueKind.String)
                {
                    name = nameElement.GetString();
                }
            }
            catch (JsonException parseException)
            {
                _logger.LogError(parseException, "Error parsing JSON request");
            }
        }

        await context.Response.WriteAsync($"Hello {name}!", context.RequestAborted);
    }
}
// [END functions_helloworld_http]
