// Copyright 2026 Google LLC
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

// [START storage_create_bucket_ip_filter]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class CreateBucketWithIpFilterSample
{
    public Bucket CreateBucketWithIpFilter(
        string projectId = "your-project-id",
        string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.CreateBucket(projectId,
            new Bucket
            {
                Name = bucketName,
                IpFilter = new Bucket.IpFilterData
                {
                    Mode = "Disabled",
                    PublicNetworkSource = new Bucket.IpFilterData.PublicNetworkSourceData
                    {
                        AllowedIpCidrRanges = new List<string>
                     {
                         "203.0.113.0/24",
                         "198.51.100.10/32"
                     }
                    },
                    AllowAllServiceAgentAccess = true,
                    AllowCrossOrgVpcs = true
                }
            });
        Console.WriteLine($"Created Bucket {bucketName} with Ip Filtering Rules");
        return bucket;
    }
}
// [END storage_create_bucket_ip_filter]
