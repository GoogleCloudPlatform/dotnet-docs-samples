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

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Samples.Run.MarkdownPreview.Renderer.Tests;

public class PostHandlerTests
{
    [Fact]
    public async Task HandleRequest_ValidMarkdown_ReturnsHtml()
    {
        var app = new WebApplicationFactory<Program>();
        var client = app.CreateClient();
        
        var markdownRequestBody = 
@"# Markdown Example
- this is a markdown list
- it should be converted to an unordered html list
";

        var expectedResponse = 
@"<h1>Markdown Example</h1>
<ul>
<li>this is a markdown list</li>
<li>it should be converted to an unordered html list</li>
</ul>
";
        var response = await client.PostAsync("/", new StringContent(markdownRequestBody, Encoding.UTF8, "application/json"));
        var actualResponse = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedResponse, actualResponse, ignoreLineEndingDifferences: true);
    }
    
    [Fact]
    public async Task HandleRequest_EmptyRequest_ReturnsEmptyResponse()
    {
        var app = new WebApplicationFactory<Program>();
        var client = app.CreateClient();
        
        var markdownRequestBody = "";
        var expectedResponse = @"";

        var response = await client.PostAsync("/", new StringContent(markdownRequestBody, Encoding.UTF8, "application/json"));
        var actualResponse = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedResponse, actualResponse);
    }
}
