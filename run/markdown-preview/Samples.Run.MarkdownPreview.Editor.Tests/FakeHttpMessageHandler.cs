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

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.Run.MarkdownPreview.Editor.Tests;

// Fake handler that captures the request for assertions and returns a specified response.
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _responseMessage;
    public HttpRequestMessage? Request { get; private set; }

    public FakeHttpMessageHandler(HttpResponseMessage responseMessage)
    {
        _responseMessage = responseMessage;
    }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Request = request;
        return Task.FromResult(_responseMessage);
    }
}
