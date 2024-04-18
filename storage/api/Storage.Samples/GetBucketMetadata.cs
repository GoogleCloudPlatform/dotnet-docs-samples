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

// [START storage_get_bucket_metadata]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class GetBucketMetadataSample
{
    public Bucket GetBucketMetadata(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName, new GetBucketOptions { Projection = Projection.Full });
        Console.WriteLine($"Bucket:\t{bucket.Name}");
        Console.WriteLine($"Acl:\t{bucket.Acl}");
        Console.WriteLine($"Billing:\t{bucket.Billing}");
        Console.WriteLine($"Cors:\t{bucket.Cors}");
        Console.WriteLine($"DefaultEventBasedHold:\t{bucket.DefaultEventBasedHold}");
        Console.WriteLine($"DefaultObjectAcl:\t{bucket.DefaultObjectAcl}");
        Console.WriteLine($"Encryption:\t{bucket.Encryption}");
        if (bucket.Encryption != null)
        {
            Console.WriteLine($"KmsKeyName:\t{bucket.Encryption.DefaultKmsKeyName}");
        }
        Console.WriteLine($"Id:\t{bucket.Id}");
        Console.WriteLine($"Kind:\t{bucket.Kind}");
        Console.WriteLine($"Lifecycle:\t{bucket.Lifecycle}");
        Console.WriteLine($"Location:\t{bucket.Location}");
        Console.WriteLine($"LocationType:\t{bucket.LocationType}");
        Console.WriteLine($"Logging:\t{bucket.Logging}");
        Console.WriteLine($"Metageneration:\t{bucket.Metageneration}");
        Console.WriteLine($"ObjectRetention:\t{bucket.ObjectRetention}");
        Console.WriteLine($"Owner:\t{bucket.Owner}");
        Console.WriteLine($"ProjectNumber:\t{bucket.ProjectNumber}");
        Console.WriteLine($"RetentionPolicy:\t{bucket.RetentionPolicy}");
        Console.WriteLine($"SelfLink:\t{bucket.SelfLink}");
        Console.WriteLine($"StorageClass:\t{bucket.StorageClass}");
        Console.WriteLine($"TimeCreated:\t{bucket.TimeCreated}");
        Console.WriteLine($"Updated:\t{bucket.Updated}");
        Console.WriteLine($"Versioning:\t{bucket.Versioning}");
        Console.WriteLine($"Website:\t{bucket.Website}");
        Console.WriteLine($"TurboReplication:\t{bucket.Rpo}");
        if (bucket.Labels != null)
        {
            Console.WriteLine("Labels:");
            foreach (var label in bucket.Labels)
            {
                Console.WriteLine($"{label.Key}:\t{label.Value}");
            }
        }
        return bucket;
    }
}
// [END storage_get_bucket_metadata]
