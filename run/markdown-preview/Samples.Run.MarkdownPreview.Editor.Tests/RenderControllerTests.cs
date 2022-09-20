// Copyright 2022 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Samples.Run.MarkdownPreview.Editor.Tests;

public class RenderControllerTests
{
    [Fact]
    public void Constructor_NoUpstreamUrl_ThrowsArgumentNull()
    {
        var emptyConfig = new Dictionary<string, string> { };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(emptyConfig)
            .Build();

        Assert.Throws<ArgumentNullException>(() => new RenderController(configuration, new HttpClient()));
    }

    [Fact]
    public async Task Render_CallsAuthenticatedUpstreamAndReturnsResponse()
    {
        var upstreamUrl = "http://some.upstream.url/";
        var configSettings = new Dictionary<string, string>
        {
            {"EDITOR_UPSTREAM_RENDER_URL", upstreamUrl}
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configSettings)
            .Build();

        var responseBody = "<h1>Hello World</h1>";
        var requestMarkdown = "# Hello World";
        var stringResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseBody)
        };
        var messageHandler = new FakeHttpMessageHandler(stringResponse);
        var client = new HttpClient(messageHandler);

        var controller = new RenderController(configuration, client);
        var result = await controller.Index(new RenderModel() {Data = requestMarkdown}) as ContentResult;
        var httpRequest = messageHandler.Request;

        // Verify response from controller
        Assert.NotNull(result);
        Assert.Equal(responseBody, result.Content);

        // Verify upstream request url and passes authorization
        Assert.NotNull(httpRequest?.Headers?.Authorization?.Parameter);
        Assert.NotEmpty(httpRequest.Headers.Authorization.Parameter);
        Assert.Equal(httpRequest.RequestUri.AbsoluteUri, upstreamUrl);
    }
}
