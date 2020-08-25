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

// [START storage_create_hmac_key]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class CreateHmacKeySample
{
    public HmacKey CreateHmacKey(
        string projectId = "your-project-id",
        string serviceAccountEmail = "dev@iam.gserviceaccount.com")
    {
        var storage = StorageClient.Create();
        var key = storage.CreateHmacKey(projectId, serviceAccountEmail);

        var secret = key.Secret;
        var metadata = key.Metadata;

        Console.WriteLine($"The Base64 encoded secret is: {secret}");
        Console.WriteLine("Make sure to save that secret, there's no API to recover it.");
        Console.WriteLine("The HMAC key metadata is:");
        Console.WriteLine($"ID: {metadata.Id}");
        Console.WriteLine($"Access ID: {metadata.AccessId}");
        Console.WriteLine($"Project ID: {metadata.ProjectId}");
        Console.WriteLine($"Service Account Email: {metadata.ServiceAccountEmail}");
        Console.WriteLine($"State: {metadata.State}");
        Console.WriteLine($"Time Created: {metadata.TimeCreated}");
        Console.WriteLine($"Time Updated: {metadata.Updated}");
        Console.WriteLine($"ETag: {metadata.ETag}");
        return key;
    }
}
// [END storage_create_hmac_key]
