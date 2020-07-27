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

// [START storage_generate_signed_url]

using Google.Cloud.Storage.V1;
using System;

public class GenerateSignedUrlSample
{
    /// <summary>
    /// Creates a signed URL which can be used to provide limited access to specific
    /// buckets and objects to anyone in possession of the URL, regardless of whether
    /// they have a Google account.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object within the bucket.</param>
    public string GenerateSignedUrl(string bucketName = "your-unique-bucket-name", string objectName = "your-object-name")
    {
        UrlSigner urlSigner = UrlSigner.FromServiceAccountPath(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
        string url = urlSigner.Sign(bucketName, objectName, TimeSpan.FromHours(1), null);
        Console.WriteLine(url);
        return url;
    }
}
// [END storage_generate_signed_url]
