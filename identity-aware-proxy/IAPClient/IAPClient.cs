/*
Copyright 2018 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

// [START iap_make_request]

using Google.Apis.Auth.OAuth2;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

public class IAPClient
{
    /// <summary>
    /// Makes a request to a IAP secured application by first obtaining
    /// an OIDC token.
    /// </summary>
    /// <param name="iapClientId">The client ID observed on 
    /// https://console.cloud.google.com/apis/credentials. </param>
    /// <param name="uri">HTTP URI to fetch.</param>
    /// <param name="cancellationToken">The token to propagate operation cancel notifications.</param>
    /// <returns>The HTTP response message.</returns>
    public async Task<HttpResponseMessage> InvokeRequestAsync(
        string iapClientId, string uri, CancellationToken cancellationToken = default)
    {
        // Get the OidcToken.
        // You only need to do this once in your application
        // as long as you can keep a reference to the returned OidcToken.
        OidcToken oidcToken = await GetOidcTokenAsync(iapClientId, cancellationToken);

        // Before making an HTTP request, always obtain the string token from the OIDC token,
        // the OIDC token will refresh the string token if it expires.
        string token = await oidcToken.GetAccessTokenAsync(cancellationToken);

        // Include the OIDC token in an Authorization: Bearer header to 
        // IAP-secured resource
        // Note: Normally you would use an HttpClientFactory to build the httpClient.
        // For simplicity we are building the HttpClient directly.
        using HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await httpClient.GetAsync(uri, cancellationToken);
    }

    /// <summary>
    /// Obtains an OIDC token for authentication an IAP request.
    /// </summary>
    /// <param name="iapClientId">The client ID observed on 
    /// https://console.cloud.google.com/apis/credentials. </param>
    /// <param name="cancellationToken">The token to propagate operation cancel notifications.</param>
    /// <returns>The HTTP response message.</returns>
    public async Task<OidcToken> GetOidcTokenAsync(string iapClientId, CancellationToken cancellationToken)
    {
        // Obtain the application default credentials.
        GoogleCredential credential = await GoogleCredential.GetApplicationDefaultAsync(cancellationToken);

        // Request an OIDC token for the Cloud IAP-secured client ID.
       return await credential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(iapClientId), cancellationToken);
    }
}

// [END iap_make_request]
