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
    /// <summary>
    /// Removes the IAM policy information about a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="role">The role to which members belong.</param>
    /// <param name="member">A collection of identifiers for members who may assume the provided role.</param>
    public void RemoveBucketIamMember(
        string bucketName = "your-unique-bucket-name",
        string role = "roles/storage.objectViewer",
        string member = "serviceAccount:dev@iam.gserviceaccount.com")
    {
        var storage = StorageClient.Create();
        var policy = storage.GetBucketIamPolicy(bucketName);
        policy.Bindings.ToList().ForEach(response =>
        {
            if (response.Role == role)
            {
                // Remove the role/member combo from the IAM policy.
                response.Members = response.Members.Where(m => m != member).ToList();
                // Remove role if it contains no members.
                if (response.Members.Count == 0)
                {
                    policy.Bindings.Remove(response);
                }
            }
        });
        // Set the modified IAM policy to be the current IAM policy.
        storage.SetBucketIamPolicy(bucketName, policy);
        Console.WriteLine($"Removed {member} with role {role} from {bucketName}");
    }
}
// [END storage_remove_bucket_iam_member]
