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

// [START storage_remove_bucket_conditional_iam_binding]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Linq;

public class RemoveBucketConditionalIamBindingSample
{
    public Policy RemoveBucketConditionalIamBinding(
        string bucketName = "your-unique-bucket-name",
        string role = "roles/storage.objectViewer",
        string title = "title",
        string description = "description",
        string expression = "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")")
    {
        var storage = StorageClient.Create();
        var policy = storage.GetBucketIamPolicy(bucketName, new GetBucketIamPolicyOptions
        {
            RequestedPolicyVersion = 3
        });
        // Set the policy schema version. For more information, please refer to https://cloud.google.com/iam/docs/policies#versions.
        policy.Version = 3;

        var bindingsToRemove = policy.Bindings.Where(binding => binding.Role == role
              && binding.Condition != null
              && binding.Condition.Title == title
              && binding.Condition.Description == description
              && binding.Condition.Expression == expression).ToList();
        if (bindingsToRemove.Count() > 0)
        {
            foreach (var binding in bindingsToRemove)
            {
                policy.Bindings.Remove(binding);
            }
            // Set the modified IAM policy to be the current IAM policy.
            policy = storage.SetBucketIamPolicy(bucketName, policy);
            Console.WriteLine("Conditional Binding was removed.");
        }
        else
        {
            Console.WriteLine("No matching conditional binding found.");
        }
        return policy;
    }
}
// [END storage_remove_bucket_conditional_iam_binding]
