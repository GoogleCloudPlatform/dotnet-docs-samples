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

// [START functions_verify_webhook]
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SlackKnowledgeGraphSearch;

public class SlackRequestVerifier
{
    private const string Version = "v0";
    private const string SignaturePrefix = Version + "=";
    private static readonly byte[] VersionBytes = Encoding.UTF8.GetBytes(Version);
    private static readonly byte[] ColonBytes = Encoding.UTF8.GetBytes(":");

    private readonly byte[] _signingKeyBytes;
    private readonly TimeSpan _timestampTolerance;

    public SlackRequestVerifier(string signingKey, TimeSpan timestampTolerance) =>
        (_signingKeyBytes, _timestampTolerance) =
        (Encoding.UTF8.GetBytes(signingKey), timestampTolerance);

    /// <summary>
    /// Verifies a request signature with the given content, which would
    /// normally have been read from the request beforehand.
    /// </summary>
    public bool VerifyRequest(HttpRequest request, byte[] content)
    {
        if (!request.Headers.TryGetValue("X-Slack-Request-Timestamp", out var timestampHeader) ||
            !request.Headers.TryGetValue("X-Slack-Signature", out var signatureHeader))
        {
            return false;
        }

        // Validate that the request isn't too old.
        if (!int.TryParse(timestampHeader, out var unixSeconds))
        {
            return false;
        }
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
        if (timestamp + _timestampTolerance < DateTimeOffset.UtcNow)
        {
            return false;
        }

        // Hash the version:timestamp:content
        using var hmac = new HMACSHA256(_signingKeyBytes);
        AddToHash(VersionBytes);
        AddToHash(ColonBytes);
        AddToHash(Encoding.UTF8.GetBytes(timestampHeader));
        AddToHash(ColonBytes);
        byte[] hash = hmac.ComputeHash(content);
        void AddToHash(byte[] bytes) => hmac.TransformBlock(bytes, 0, bytes.Length, null, 0);

        // Compare the resulting signature with the signature in the request.
        return CreateSignature(hash) == (string) signatureHeader;
    }

    private const string Hex = "0123456789abcdef";
    private static string CreateSignature(byte[] hash)
    {
        var builder = new StringBuilder(SignaturePrefix, SignaturePrefix.Length + hash.Length * 2);
        foreach (var b in hash)
        {
            builder.Append(Hex[b >> 4]);
            builder.Append(Hex[b & 0xf]);
        }
        return builder.ToString();
    }
}
// [END functions_verify_webhook]
