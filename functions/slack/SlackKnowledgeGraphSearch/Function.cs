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

using Google.Apis.Kgsearch.v1;
using Google.Apis.Kgsearch.v1.Data;
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SlackKnowledgeGraphSearch;

public class Function : IHttpFunction
{
    // [START functions_slack_search]
    private readonly ILogger _logger;
    private readonly KgsearchService _kgService;
    private readonly SlackRequestVerifier _verifier;

    public Function(ILogger<Function> logger, KgsearchService kgService, SlackRequestVerifier verifier) =>
        (_logger, _kgService, _verifier) = (logger, kgService, verifier);

    public async Task HandleAsync(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;
        var cancellationToken = context.RequestAborted;

        // Validate request
        if (request.Method != "POST")
        {
            _logger.LogWarning("Unexpected request method '{method}'", request.Method);
            response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
            return;
        }

        if (!request.HasFormContentType)
        {
            _logger.LogWarning("Unexpected content type '{contentType}'", request.ContentType);
            response.StatusCode = (int) HttpStatusCode.BadRequest;
            return;
        }

        // We need to read the request body twice: once to validate the signature,
        // and once to read the form content. We copy it into a memory stream,
        // so that we can rewind it after reading.
        var bodyCopy = new MemoryStream();
        await request.Body.CopyToAsync(bodyCopy, cancellationToken);
        request.Body = bodyCopy;
        bodyCopy.Position = 0;

        if (!_verifier.VerifyRequest(request, bodyCopy.ToArray()))
        {
            _logger.LogWarning("Slack request verification failed");
            response.StatusCode = (int) HttpStatusCode.Unauthorized;
            return;
        }

        var form = await request.ReadFormAsync();
        if (!form.TryGetValue("text", out var query))
        {
            _logger.LogWarning("Slack request form did not contain a text element");
            response.StatusCode = (int) HttpStatusCode.BadRequest;
            return;
        }

        var kgResponse = await SearchKnowledgeGraphAsync(query, cancellationToken);
        string formattedResponse = FormatSlackMessage(kgResponse, query);
        response.ContentType = "application/json";
        await response.WriteAsync(formattedResponse);
    }
    // [END functions_slack_search]

    // [START functions_slack_request]
    private async Task<SearchResponse> SearchKnowledgeGraphAsync(string query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Performing Knowledge Graph search for '{query}'", query);
        var request = _kgService.Entities.Search();
        request.Limit = 1;
        request.Query = query;
        return await request.ExecuteAsync(cancellationToken);
    }
    // [END functions_slack_request]

    // [START functions_slack_format]
    private string FormatSlackMessage(SearchResponse kgResponse, string query)
    {
        JObject attachment = new JObject();
        JObject response = new JObject();

        response["response_type"] = "in_channel";
        response["text"] = $"Query: {query}";

        var element = kgResponse.ItemListElement?.FirstOrDefault() as JObject;
        if (element is object && element.TryGetValue("result", out var entityToken) &&
            entityToken is JObject entity)
        {
            string title = (string) entity["name"];
            if (entity.TryGetValue("description", out var description))
            {
                title = $"{title}: {description}";
            }
            attachment["title"] = title;
            if (entity.TryGetValue("detailedDescription", out var detailedDescriptionToken) &&
                detailedDescriptionToken is JObject detailedDescription)
            {
                AddPropertyIfPresent(detailedDescription, "url", "title_link");
                AddPropertyIfPresent(detailedDescription, "articleBody", "text");
            }
            if (entity.TryGetValue("image", out var imageToken) &&
                imageToken is JObject image)
            {
                AddPropertyIfPresent(image, "contentUrl", "image_url");
            }
        }
        else
        {
            attachment["text"] = "No results match your query...";
        }
        response["attachments"] = new JArray { attachment };
        return response.ToString();

        void AddPropertyIfPresent(JObject parent, string sourceProperty, string targetProperty)
        {
            if (parent.TryGetValue(sourceProperty, out var propertyValue))
            {
                attachment[targetProperty] = propertyValue;
            }
        }
    }
    // [END functions_slack_format]
}
