// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START storage_restore_bucket]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.Control.V2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Storage.Samples
{
    public class RestoreSoftDeletedBucketSample
    {
        public Bucket RestoreSoftDeletedBucket(string bucketName = "your-unique-bucket-name")
        {
            var client = StorageClient.Create();
            var bucket = client.GetBucket(bucketName, new GetBucketOptions { SoftDeleted = false });
            client.DeleteBucket(bucket.Name, new DeleteBucketOptions { DeleteObjects = true });
            var restored = client.RestoreBucket(bucket.Name, bucket.Generation.Value);
            Console.WriteLine($"Bucket Name:\t {restored.Name}");
            Console.WriteLine($"Bucket Generation:\t {restored.Generation}");
            return restored;
        }
    }
}
// [END storage_restore_bucket]
