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

using System.Net.Http.Headers;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;

public class RenderController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly string _editorUpstreamRenderUrl;

    public RenderController(IConfiguration config, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _editorUpstreamRenderUrl = config["EDITOR_UPSTREAM_RENDER_URL"];

        if (string.IsNullOrEmpty(_editorUpstreamRenderUrl))
        {
            throw new ArgumentNullException("EDITOR_UPSTREAM_RENDER_URL",
                "Missing configuration for upstream render service: add EDITOR_UPSTREAM_RENDER_URL as environment variable " +
                "or in appsettings.json.");
        }
    }

    // [START cloudrun_secure_request_do]
    public async Task<IActionResult> Index([FromBody] RenderModel model)
    {
        var markdown = model.Data ?? string.Empty;
        var renderedHtml = await GetAuthenticatedPostResponse(_editorUpstreamRenderUrl, markdown);
        return Content(renderedHtml);
    }
    // [END cloudrun_secure_request_do]

    // [START cloudrun_secure_request]
    private async Task<string> GetAuthenticatedPostResponse(string url, string postBody)
    {
        // Get the OIDC access token from the service account via Application Default Credentials
        GoogleCredential credential = await GoogleCredential.GetApplicationDefaultAsync();  
        OidcToken token = await credential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(url));
        string accessToken = await token.GetAccessTokenAsync();
        
        // Create request to the upstream service with the generated OAuth access token in the Authorization header
        var upstreamRequest = new HttpRequestMessage(HttpMethod.Post, url);
        upstreamRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        upstreamRequest.Content = new StringContent(postBody);
        
        var upstreamResponse = await _httpClient.SendAsync(upstreamRequest);
        upstreamResponse.EnsureSuccessStatusCode();
        
        return await upstreamResponse.Content.ReadAsStringAsync();
    }
    // [END cloudrun_secure_request]
}
