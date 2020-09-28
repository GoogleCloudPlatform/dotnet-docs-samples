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

// [START storage_remove_bucket_iam_member]

using Google.Cloud.Storage.V1;
using System;
using System.Linq;

public class RemoveBucketIamMemberSample
{
    public void RemoveBucketIamMember(
        string bucketName = "your-unique-bucket-name",
        string role = "roles/storage.objectViewer",
        string member = "serviceAccount:dev@iam.gserviceaccount.com")
    {
        var storage = StorageClient.Create();
        var policy = storage.GetBucketIamPolicy(bucketName, new GetBucketIamPolicyOptions
        {
            RequestedPolicyVersion = 3
        });
        // Set the policy schema version. For more information, please refer to https://cloud.google.com/iam/docs/policies#versions.
        policy.Version = 3;

        foreach (var binding in policy.Bindings.Where(c => c.Role == role).ToList())
        {
            // Remove the role/member combo from the IAM policy.
            binding.Members = binding.Members.Where(m => m != member).ToList();
            // Remove role if it contains no members.
            if (binding.Members.Count == 0)
            {
                policy.Bindings.Remove(binding);
            }
        }
        // Set the modified IAM policy to be the current IAM policy.
        storage.SetBucketIamPolicy(bucketName, policy);
        Console.WriteLine($"Removed {member} with role {role} from {bucketName}");
    }
}
// [END storage_remove_bucket_iam_member]
