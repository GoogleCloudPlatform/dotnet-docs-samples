/*
 * Copyright (c) 2017 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using CommandLine;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SignUrlSample
{
    [Verb("signurls")]
    class SignedUrlArgs
    {
        [Value(0, HelpText = "Url to sign", Required = true)]
        public string Url { get; set; }

        [Option('k', "key-name",
            HelpText = "The name of the key to use when encrypting url")]
        public string KeyName { get; set; }

        [Option('p', "key-path",
            HelpText = "The path to the key to use when encrypting url")]
        public string KeyPath { get; set; }

        [Option('e', "expiration",
            HelpText = "Url expiration in UTC formatted as: YYYY-MM-DDThh:mm:ss")]
        public DateTime Expiration { get; set; }
    }

    public class Program
    {
        static int Main(string[] args)
        {
            string signedUrl = null;
            Parser.Default.ParseArguments<SignedUrlArgs>(args).MapResult(
                uargs =>
                    signedUrl = CreateSignedUrl(
                                    uargs.Url,
                                    uargs.KeyName,
                                    File.ReadAllText(uargs.KeyPath, Encoding.UTF8),
                                    uargs.Expiration),
                err => string.Empty);

            if (string.IsNullOrEmpty(signedUrl))
            {
                return 1;
            }

            Console.WriteLine($"Signed URL: {signedUrl}");
            return 0;
        }

        // [START CreateSignedUrl]
        /// <summary>
        /// Creates signed URL for Google Cloud SDN
        /// More details about order of operations is here: 
        /// <see cref="https://cloud-dot-devsite.googleplex.com/cdn/docs/signed-urls#creatingkeys"/>
        /// </summary>
        /// <param name="url">The Url to sign. This URL can't include Expires and KeyName query parameters in it</param>
        /// <param name="keyName">The name of the key used to sign the URL</param>
        /// <param name="encodedKey">The key used to sign the Url</param>
        /// <param name="expirationTime">Expiration time of the signature</param>
        /// <returns>Signed Url that is valid until {expirationTime}</returns>
        public static string CreateSignedUrl(string url, string keyName, string encodedKey, DateTime expirationTime)
        {
            var builder = new UriBuilder(url);

            long unixTimestampExpiration = ToUnixTime(expirationTime);

            char queryParam = string.IsNullOrEmpty(builder.Query) ? '?' : '&';
            builder.Query += $"{queryParam}Expires={unixTimestampExpiration}&KeyName={keyName}".ToString();

            // Key is passed as base64url encoded
            byte[] decodedKey = Base64UrlDecode(encodedKey);

            // Computes HMAC SHA-1 hash of the URL using the key
            byte[] hash = ComputeHash(decodedKey, builder.Uri.AbsoluteUri);
            string encodedHash = Base64UrlEncode(hash);

            builder.Query += $"&Signature={encodedHash}";
            return builder.Uri.AbsoluteUri;
        }

        private static long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        private static byte[] Base64UrlDecode(string arg)
        {
            string s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding

            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        private static string Base64UrlEncode(byte[] inputBytes)
        {
            var output = Convert.ToBase64String(inputBytes);

            output = output.Replace('+', '-')      // 62nd char of encoding
                           .Replace('/', '_');     // 63rd char of encoding

            return output;
        }

        private static byte[] ComputeHash(byte[] secretKey, string signatureString)
        {
            var enc = Encoding.ASCII;
            using (HMACSHA1 hmac = new HMACSHA1(secretKey))
            {
                hmac.Initialize();

                byte[] buffer = enc.GetBytes(signatureString);

                return hmac.ComputeHash(buffer);
            }
        }

        // [END CreateSignedUrl]
    }
}
