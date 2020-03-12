// Copyright 2020 Google Inc.
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

namespace Storage
{
    public class GenerateSignedUrl
    {
        // [START storage_generate_signed_url]
        public static void StorageGenerateSignedUrl(string bucketName, string objectName)
        {
            UrlSigner urlSigner = UrlSigner.FromServiceAccountPath(Environment
                .GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            string url =
                urlSigner.Sign(bucketName, objectName, TimeSpan.FromHours(1), null);
            Console.WriteLine(url);
        }
        // [END storage_generate_signed_url]
    }
}
