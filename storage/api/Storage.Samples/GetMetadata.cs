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

// [START storage_get_metadata]

using Google.Cloud.Storage.V1;
using System;

public class GetMetadataSample
{
    public Google.Apis.Storage.v1.Data.Object GetMetadata(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name")
    {
        var storage = StorageClient.Create();
        var storageObject = storage.GetObject(bucketName, objectName, new GetObjectOptions { Projection = Projection.Full });
        Console.WriteLine($"Bucket:\t{storageObject.Bucket}");
        Console.WriteLine($"CacheControl:\t{storageObject.CacheControl}");
        Console.WriteLine($"ComponentCount:\t{storageObject.ComponentCount}");
        Console.WriteLine($"ContentDisposition:\t{storageObject.ContentDisposition}");
        Console.WriteLine($"ContentEncoding:\t{storageObject.ContentEncoding}");
        Console.WriteLine($"ContentLanguage:\t{storageObject.ContentLanguage}");
        Console.WriteLine($"ContentType:\t{storageObject.ContentType}");
        Console.WriteLine($"Crc32c:\t{storageObject.Crc32c}");
        Console.WriteLine($"ETag:\t{storageObject.ETag}");
        Console.WriteLine($"Generation:\t{storageObject.Generation}");
        Console.WriteLine($"Id:\t{storageObject.Id}");
        Console.WriteLine($"Kind:\t{storageObject.Kind}");
        Console.WriteLine($"KmsKeyName:\t{storageObject.KmsKeyName}");
        Console.WriteLine($"Md5Hash:\t{storageObject.Md5Hash}");
        Console.WriteLine($"MediaLink:\t{storageObject.MediaLink}");
        Console.WriteLine($"Metageneration:\t{storageObject.Metageneration}");
        Console.WriteLine($"Name:\t{storageObject.Name}");
        Console.WriteLine($"Retention:\t{storageObject.Retention}");
        Console.WriteLine($"Size:\t{storageObject.Size}");
        Console.WriteLine($"StorageClass:\t{storageObject.StorageClass}");
        Console.WriteLine($"TimeCreated:\t{storageObject.TimeCreated}");
        Console.WriteLine($"Updated:\t{storageObject.Updated}");
        bool eventBasedHold = storageObject.EventBasedHold ?? false;
        Console.WriteLine("Event-based hold enabled? {0}", eventBasedHold);
        bool temporaryHold = storageObject.TemporaryHold ?? false;
        Console.WriteLine("Temporary hold enabled? {0}", temporaryHold);
        Console.WriteLine($"RetentionExpirationTime\t{storageObject.RetentionExpirationTime}");
        if (storageObject.Metadata != null)
        {
            Console.WriteLine("Metadata: ");
            foreach (var metadata in storageObject.Metadata)
            {
                Console.WriteLine($"{metadata.Key}:\t{metadata.Value}");
            }
        }
        Console.WriteLine($"CustomTime:\t{storageObject.CustomTime}");
        return storageObject;
    }
}
// [END storage_get_metadata]
