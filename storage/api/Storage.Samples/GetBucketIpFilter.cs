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

// [START storage_get_ip_filtering]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class GetBucketIpFilterSample
{
    /// <summary>
    /// Retrieve the IP filtering rules for the bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    public Bucket.IpFilterData GetBucketIpFilter(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);

        if (bucket.IpFilter == null)
        {
            Console.WriteLine($"Bucket {bucketName} has no IP Filter configured.");
            return null;
        }

        Console.WriteLine($"IP Filter Configuration for the Bucket {bucketName}:");
        Console.WriteLine($"Mode: {bucket.IpFilter.Mode}");
        Console.WriteLine($"Allow All Service Agent Access: {bucket.IpFilter.AllowAllServiceAgentAccess}");

        if (bucket.IpFilter.PublicNetworkSource?.AllowedIpCidrRanges != null)
        {
            Console.WriteLine("Allowed Public CIDR Ranges:");
            foreach (var range in bucket.IpFilter.PublicNetworkSource.AllowedIpCidrRanges)
            {
                Console.WriteLine($"- {range}");
            }
        }
        Console.WriteLine($"Allow Cross Organization VPCs Access: {bucket.IpFilter.AllowCrossOrgVpcs}");

        if (bucket.IpFilter?.VpcNetworkSources != null)
        {
            Console.WriteLine("Allowed VPC Network:");
            foreach (var vpcNetwork in bucket.IpFilter.VpcNetworkSources)
            {
                Console.WriteLine($"- Network: {vpcNetwork.Network}");
                if (vpcNetwork.AllowedIpCidrRanges != null)
                {
                    Console.WriteLine($"Allowed VPC CIDR Ranges: {string.Join(", ", vpcNetwork.AllowedIpCidrRanges)}");
                }
            }
        }
        return bucket.IpFilter;
    }
}
// [END storage_get_ip_filtering]
