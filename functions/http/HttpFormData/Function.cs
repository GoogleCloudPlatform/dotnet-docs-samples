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

// [START functions_http_form_data]
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace HttpFormData;

public class Function : IHttpFunction
{
    private readonly ILogger _logger;

    public Function(ILogger<Function> logger) =>
        _logger = logger;

    public async Task HandleAsync(HttpContext context)
    {
        HttpResponse response = context.Response;
        HttpRequest request = context.Request;

        if (request.Method != "POST")
        {
            response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
            return;
        }

        // This code will process each file uploaded.
        string tempDirectory = Path.GetTempPath();
        foreach (IFormFile file in request.Form.Files)
        {
            if (string.IsNullOrEmpty(file.FileName))
            {
                continue;
            }
            _logger.LogInformation("Processed file: {file}", file.FileName);

            // Note: GCF's temp directory is an in-memory file system
            // Thus, any files in it must fit in the instance's memory.
            string outputPath = Path.Combine(tempDirectory, file.FileName);

            // Note: files saved to a GCF instance itself may not persist across executions.
            // Persistent files should be stored elsewhere, e.g. a Cloud Storage bucket.
            using (FileStream output = File.Create(outputPath))
            {
                await file.CopyToAsync(output, context.RequestAborted);
            }

            // TODO(developer): process saved files here
            File.Delete(outputPath);
        }

        // This code will process other form fields.
        foreach (KeyValuePair<string, StringValues> parameter in request.Form)
        {
            // TODO(developer): process field values here
            _logger.LogInformation("Processed field '{key}' (value: '{value}')",
                parameter.Key, (string) parameter.Value);
        }
    }
}
// [END functions_http_form_data]
