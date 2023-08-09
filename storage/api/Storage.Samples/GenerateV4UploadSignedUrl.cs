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

// [START storage_generate_upload_signed_url_v4]

using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Net.Http;

public class GenerateV4UploadSignedUrlSample
{
    public string GenerateV4UploadSignedUrl(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name")
    {
        UrlSigner urlSigner = UrlSigner.FromCredential(GoogleCredential.GetApplicationDefault());

        var contentHeaders = new Dictionary<string, IEnumerable<string>>
        {
            { "Content-Type", new[] { "text/plain" } }
        };

        // V4 is the default signing version.
        UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromHours(1));

        UrlSigner.RequestTemplate template = UrlSigner.RequestTemplate
            .FromBucket(bucketName)
            .WithObjectName(objectName)
            .WithHttpMethod(HttpMethod.Put)
            .WithContentHeaders(contentHeaders);

        string url = urlSigner.Sign(template, options);
        Console.WriteLine("Generated PUT signed URL:");
        Console.WriteLine(url);
        Console.WriteLine("You can use this URL with any user agent, for example:");
        Console.WriteLine($"curl -X PUT -H 'Content-Type: text/plain' --upload-file my-file '{url}'");
        return url;
    }
}
// [END storage_generate_upload_signed_url_v4]
