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

// [START storage_enable_ip_filtering]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;

public class EnableBucketIpFilterSample
{
    /// <summary>
    ///  Create or update IP filtering rules for the bucket.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="publicRange">The new CIDR range to add (e.g., "1.2.3.4/32") in the public network.</param>
    /// <param name="vpcRange">The new CIDR range to add (e.g., "10.0.0.0/24") in the VPC network.</param>
    /// <param name="vpcNetwork">The name of the vpc network. (e.g., "NETWORK_NAME" from the network resource name : projects/{PROJECT_ID}/global/networks/{NETWORK_NAME})</param>
    public Bucket EnableBucketIpFilter(string projectId = "your-project-id", string bucketName = "your-unique-bucket-name", string publicRange = "192.0.2.0/24",
        string vpcRange = "10.0.0.0/24", string vpcNetwork = "default")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);

        if (bucket.IpFilter == null)
        {
            bucket.IpFilter = new Bucket.IpFilterData
            {
                Mode = "Enabled",
                PublicNetworkSource = new Bucket.IpFilterData.PublicNetworkSourceData
                {
                    AllowedIpCidrRanges = new List<string>()
                },
                VpcNetworkSources = new List<Bucket.IpFilterData.VpcNetworkSourcesData>(),
                AllowAllServiceAgentAccess = true,
                AllowCrossOrgVpcs = true
            };
        }
        else
        {
            bucket.IpFilter.Mode = "Enabled";
            bucket.IpFilter.AllowAllServiceAgentAccess ??= true;
            bucket.IpFilter.AllowCrossOrgVpcs ??= true;
        }

        bucket.IpFilter.PublicNetworkSource ??= new Bucket.IpFilterData.PublicNetworkSourceData();
        bucket.IpFilter.PublicNetworkSource.AllowedIpCidrRanges ??= new List<string>();

        if (!bucket.IpFilter.PublicNetworkSource.AllowedIpCidrRanges.Contains(publicRange))
        {
            bucket.IpFilter.PublicNetworkSource.AllowedIpCidrRanges.Add(publicRange);
        }

        bucket.IpFilter.VpcNetworkSources ??= new List<Bucket.IpFilterData.VpcNetworkSourcesData>();
        string networkName = $"projects/{projectId}/global/networks/{vpcNetwork}";
        var existingVpcNetwork = bucket.IpFilter.VpcNetworkSources.FirstOrDefault(v => v.Network == networkName);

        if (existingVpcNetwork != null)
        {
            existingVpcNetwork.AllowedIpCidrRanges ??= new List<string>();
            if (!existingVpcNetwork.AllowedIpCidrRanges.Contains(vpcRange))
            {
                existingVpcNetwork.AllowedIpCidrRanges.Add(vpcRange);
            }
        }
        else
        {
            bucket.IpFilter.VpcNetworkSources.Add(new Bucket.IpFilterData.VpcNetworkSourcesData
            {
                Network = networkName,
                AllowedIpCidrRanges = new List<string> { vpcRange }
            });
        }

        var updatedBucket = storage.UpdateBucket(bucket);

        var currentPublicRanges = updatedBucket.IpFilter?.PublicNetworkSource?.AllowedIpCidrRanges != null
                ? string.Join(", ", updatedBucket.IpFilter.PublicNetworkSource.AllowedIpCidrRanges)
                : "None";

        var currentVpcRanges = updatedBucket.IpFilter?.VpcNetworkSources != null && updatedBucket.IpFilter.VpcNetworkSources.Any()
                ? string.Join(", ", updatedBucket.IpFilter.VpcNetworkSources.SelectMany(v => v.AllowedIpCidrRanges ?? new List<string>()))
                : "None";

        Console.WriteLine($"Enabled IP filtering Rules for the Bucket: {bucketName}. " +
                $"Current Public Network CIDR Ranges: [{currentPublicRanges}]. " +
                $"Current VPC Network CIDR Ranges: [{currentVpcRanges}].");
        return updatedBucket;
    }
}
// [END storage_enable_ip_filtering]
