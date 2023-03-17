// Copyright (c) 2020 Google LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// https://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START iap_validate_jwt]

using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Threading;
using System.Threading.Tasks;

public class IAPTokenVerification
{
    /// <summary>
    /// Verifies a signed jwt token and returns its payload.
    /// </summary>
    /// <param name="signedJwt">The token to verify.</param>
    /// <param name="expectedAudience">The audience that the token should be meant for.
    /// Validation will fail if that's not the case.</param>
    /// <param name="cancellationToken">The cancellation token to propagate cancellation requests.</param>
    /// <returns>A task that when completed will have as its result the payload of the verified token.</returns>
    /// <exception cref="InvalidJwtException">If verification failed. The message of the exception will contain
    /// information as to why the token failed.</exception>
    public async Task<JsonWebSignature.Payload> VerifyTokenAsync(
        string signedJwt, string expectedAudience, CancellationToken cancellationToken = default)
    {
        SignedTokenVerificationOptions options = new SignedTokenVerificationOptions
        {
            // Use clock tolerance to account for possible clock differences
            // between the issuer and the verifier.
            IssuedAtClockTolerance = TimeSpan.FromMinutes(1),
            ExpiryClockTolerance = TimeSpan.FromMinutes(1),
            TrustedAudiences = { expectedAudience },
            TrustedIssuers = { "https://cloud.google.com/iap" },
            CertificatesUrl = GoogleAuthConsts.IapKeySetUrl,
        };
        
        return await JsonWebSignature.VerifySignedTokenAsync(signedJwt, options, cancellationToken: cancellationToken);
    }
}

// [END iap_validate_jwt]

