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

// [START storage_print_bucket_acl_for_user]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;

public class PrintBucketAclForUserSample
{
    public IEnumerable<BucketAccessControl> PrintBucketAclForUser(
        string bucketName = "your-unique-bucket-name",
        string userEmail = "dev@iam.gserviceaccount.com")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName, new GetBucketOptions { Projection = Projection.Full });

        var bucketAclForUser = bucket.Acl.Where((acl) => acl.Entity == $"user-{userEmail}");
        foreach (var acl in bucketAclForUser)
        {
            Console.WriteLine($"{acl.Role}:{acl.Entity}");
        }

        return bucketAclForUser;
    }
}
// [END storage_print_bucket_acl_for_user]
