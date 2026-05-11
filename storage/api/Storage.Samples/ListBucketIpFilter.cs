// Copyright 2026 Google LLC
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

// [START storage_list_bucket_ip_filter]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

/// <summary>
/// Retrieves the IP Filter configuration for the bucket.
/// <param name="bucketName">The name of the bucket.</param>
/// </summary>
public class ListBucketIpFiltersSample
{
    public Bucket.IpFilterData ListBucketIpFilters(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);

        if (bucket.IpFilter == null)
        {
            Console.WriteLine($"No IP Filter Configuration found for the Bucket: {bucketName}");
            return null;
        }

        Console.WriteLine($"Bucket Ip Filter Data for the {bucketName}:");

        Console.WriteLine($"- Mode: {bucket.IpFilter.Mode}");
        Console.WriteLine($"- Allow All Service Agent Access: {bucket.IpFilter.AllowAllServiceAgentAccess}");
        Console.WriteLine($"- Allow Cross-Org VPCs: {bucket.IpFilter.AllowCrossOrgVpcs}");

        if (bucket.IpFilter.PublicNetworkSource?.AllowedIpCidrRanges != null)
        {
            string ranges = string.Join(", ", bucket.IpFilter.PublicNetworkSource.AllowedIpCidrRanges);
            Console.WriteLine($"- Allowed IP CIDR Ranges: {ranges}");
        }
        return bucket.IpFilter;
    }
}
// [END storage_list_bucket_ip_filter]
