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

// [START storage_create_bucket_hierarchical_namespace]
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class CreateBucketWithHierarchicalNamespaceEnabledSample
{
    public Bucket CreateBucketWithHierarchicalNamespace(
        string projectId = "your-project-id",
        string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.CreateBucket(projectId,
            new Bucket
            {
                Name = bucketName,
                IamConfiguration = new Bucket.IamConfigurationData
                {
                    UniformBucketLevelAccess = new Bucket.IamConfigurationData.UniformBucketLevelAccessData { Enabled = true }
                },
                HierarchicalNamespace = new Bucket.HierarchicalNamespaceData { Enabled = true }
            });
        Console.WriteLine($"Created {bucketName} with Hierarchical Namespace enabled.");
        return bucket;
    }
}
// [END storage_create_bucket_hierarchical_namespace]
