// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START storage_delete_ip_filtering_rules]

using Google.Cloud.Storage.V1;
using System;
using System.Linq;

public class DeleteBucketIpFilterSample
{
    /// <summary>
    /// Delete specific IP filter rules or VPC network sources from the bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="publicRangeToDelete">The public CIDR range to delete (e.g., "1.2.3.4/32").</param>
    /// <param name="vpcNetworkToDelete">The name of the VPC network source to delete (e.g., "projects/my-project/global/networks/my-vpc").</param>
    public void DeleteBucketIpFilter(
        string bucketName = "your-unique-bucket-name",
        string publicRangeToDelete = null,
        string vpcNetworkToDelete = null)
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        bool updated = false;

        if (bucket.IpFilter == null)
        {
            Console.WriteLine($"Bucket {bucketName} has no IP Filter Configuration.");
            return;
        }

        if (!string.IsNullOrEmpty(publicRangeToDelete) &&
            bucket.IpFilter.PublicNetworkSource?.AllowedIpCidrRanges != null)
        {
            bool removedPublic = bucket.IpFilter.PublicNetworkSource.AllowedIpCidrRanges.Remove(publicRangeToDelete);
            if (removedPublic)
            {
                Console.WriteLine($"Removed Public CIDR Range {publicRangeToDelete} from the Bucket {bucketName}.");
                updated = true;
            }
            else
            {
                Console.WriteLine($"Public CIDR Range {publicRangeToDelete} not found in the Bucket {bucketName} Configuration.");
            }
        }

        if (!string.IsNullOrEmpty(vpcNetworkToDelete) && bucket.IpFilter.VpcNetworkSources != null)
        {
            var vpcToRemove = bucket.IpFilter.VpcNetworkSources
                .FirstOrDefault(v => v.Network == vpcNetworkToDelete);

            if (vpcToRemove != null)
            {
                bucket.IpFilter.VpcNetworkSources.Remove(vpcToRemove);
                Console.WriteLine($"Removed VPC Network Source {vpcNetworkToDelete} from the Bucket {bucketName}.");
                updated = true;
            }
            else
            {
                Console.WriteLine($"VPC Network {vpcNetworkToDelete} not found in the Bucket {bucketName} Configuration.");
            }
        }

        if (updated)
        {
            storage.UpdateBucket(bucket);
        }
        else
        {
            Console.WriteLine("No changes were made to the Bucket's IP filters.");
        }
    }
}
// [END storage_delete_ip_filtering_rules]
