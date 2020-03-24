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

using Google.Cloud.Storage.V1;
using System;
using System.Linq;

namespace Storage
{
    public class RemoveBucketConditionalIamBinding
    {
        // [START storage_remove_bucket_conditional_iam_binding]
        public static void StorageRemoveBucketConditionalIamBinding(string bucketName,
            string role, string title, string description, string expression)
        {
            var storage = StorageClient.Create();
            var policy = storage.GetBucketIamPolicy(bucketName, new GetBucketIamPolicyOptions()
            {
                RequestedPolicyVersion = 3
            });
            policy.Version = 3;
            if (policy.Bindings.ToList().RemoveAll(binding => binding.Role == role
                && binding.Condition != null
                && binding.Condition.Title == title
                && binding.Condition.Description == description
                && binding.Condition.Expression == expression) > 0)
            {
                // Set the modified IAM policy to be the current IAM policy.
                storage.SetBucketIamPolicy(bucketName, policy);
                Console.WriteLine("Conditional Binding was removed.");
            }
            else
            {
                Console.WriteLine("No matching conditional binding found.");
            }
        }
        // [END storage_remove_bucket_conditional_iam_binding]
    }
}
