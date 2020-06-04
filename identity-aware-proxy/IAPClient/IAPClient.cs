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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    class IAPClient
    {
        /// <summary>
        /// Authenticates using the client id and credentials, then fetches
        /// the uri.
        /// </summary>
        /// <param name="iapClientId">The client id observed on 
        /// https://console.cloud.google.com/apis/credentials. </param>
        /// <param name="credentialsFilePath">Path to the credentials .json file
        /// downloaded from https://console.cloud.google.com/apis/credentials.
        /// </param>
        /// <param name="uri">HTTP uri to fetch.</param>
        /// <returns>The http response body as a string.</returns>
        public async Task<string> InvokeRequestAsync(string iapClientId, string credentialsFilePath, string uri)
        {
            // Read credentials from the credentials .json file.
            ServiceAccountCredential saCredential;
            using (var fs = new FileStream(credentialsFilePath, FileMode.Open, FileAccess.Read))
            {
                saCredential = ServiceAccountCredential.FromServiceAccountData(fs);
            }

            // Request an OIDC token for the Cloud IAP-secured client ID.
            OidcToken oidcToken = await saCredential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(iapClientId)).ConfigureAwait(false);
            // Always request the string token from the OIDC token, the OIDC token will refresh the string token if it expires.
            string token = await oidcToken.GetAccessTokenAsync().ConfigureAwait(false);

            // Include the OIDC token in an Authorization: Bearer header to 
            // IAP-secured resource
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string response = await httpClient.GetStringAsync(uri).ConfigureAwait(false);
                return response;
            }
        }
    }
}
// [END iap_make_request]
