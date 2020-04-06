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

// [START storage_remove_bucket_owner]
using Google.Cloud.Storage.V1;
using System.Linq;

namespace Storage
{
    public class RemoveBucketOwner
    {
        public static void StorageRemoveBucketOwner(string bucketName, string userEmail)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName, new GetBucketOptions()
            {
                Projection = Projection.Full
            });
            if (null == bucket.Acl)
                return;
            bucket.Acl = bucket.Acl.Where((acl) =>
                !(acl.Entity == $"user-{userEmail}" && acl.Role == "OWNER")
                ).ToList();
            var updatedBucket = storage.UpdateBucket(bucket, new UpdateBucketOptions()
            {
                // Avoid race conditions.
                IfMetagenerationMatch = bucket.Metageneration,
            });
        }
    }
}
// [END storage_remove_bucket_owner]
