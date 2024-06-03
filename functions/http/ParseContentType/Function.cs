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

// [START functions_http_content]
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ParseContentType;

public class Function : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;

        string name = null;
        ContentType contentType = new ContentType(request.ContentType);

        switch (contentType.MediaType)
        {
            case "application/json":
            {
                // '{"name":"John"}'
                using TextReader reader = new StreamReader(request.Body);
                string json = await reader.ReadToEndAsync();
                JsonElement body = JsonSerializer.Deserialize<JsonElement>(json);
                if (body.TryGetProperty("name", out JsonElement property) && property.ValueKind == JsonValueKind.String)
                {
                    name = property.GetString();
                }
                break;
            }
            case "application/octet-stream":
            {
                // 'John', encoded to bytes using UTF-8, then encoded as base64
                using TextReader reader = new StreamReader(request.Body);
                string base64 = await reader.ReadToEndAsync();
                byte[] data = Convert.FromBase64String(base64);
                name = Encoding.UTF8.GetString(data);
                break;
            }
            case "text/plain":
            {
                // 'John'
                using TextReader reader = new StreamReader(request.Body);
                name = await reader.ReadLineAsync();
                break;
            }
            case "application/x-www-form-urlencoded":
            {
                // 'name=John' in the body of a POST request (not the URL)
                if (request.Form.TryGetValue("name", out StringValues value))
                {
                    name = value;
                }
                break;
            }
        }
        if (name is object)
        {
            await response.WriteAsync($"Hello {name}!", context.RequestAborted);
        }
        else
        {
            // Unrecognized content type, or the name wasn't in the content
            // (e.g. JSON without a "name" property)
            response.StatusCode = (int) HttpStatusCode.BadRequest;
        }
    }
}
// [END functions_http_content]
