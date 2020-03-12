﻿// Copyright 2020 Google Inc.
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

using Google.Cloud.Storage.V1;
using System;
using System.Net.Http;

namespace Storage
{
    public class GenerateV4ReadSignedUrl
    {
        // [START storage_generate_signed_url_v4]
        public static void StorageGenerateV4SignedReadUrl(string bucketName, string objectName)
        {
            UrlSigner urlSigner = UrlSigner
                .FromServiceAccountPath(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"))
                .WithSigningVersion(SigningVersion.V4);
            string url = urlSigner.Sign(bucketName, objectName, TimeSpan.FromHours(1), HttpMethod.Get);
            Console.WriteLine("Generated GET signed URL:");
            Console.WriteLine(url);
            Console.WriteLine("You can use this URL with any user agent, for example:");
            Console.WriteLine($"curl '{url}'");
        }
        // [END storage_generate_signed_url_v4]
    }
}
