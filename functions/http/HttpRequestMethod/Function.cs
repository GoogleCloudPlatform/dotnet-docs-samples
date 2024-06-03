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

// [START functions_http_method]
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace HttpRequestMethod;

public class Function : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        HttpResponse response = context.Response;
        switch (context.Request.Method)
        {
            case "GET":
                response.StatusCode = (int) HttpStatusCode.OK;
                await response.WriteAsync("Hello world!", context.RequestAborted);
                break;
            case "PUT":
                response.StatusCode = (int) HttpStatusCode.Forbidden;
                await response.WriteAsync("Forbidden!", context.RequestAborted);
                break;
            default:
                response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
                await response.WriteAsync("Something blew up!", context.RequestAborted);
                break;
        }
    }
}
// [END functions_http_method]
