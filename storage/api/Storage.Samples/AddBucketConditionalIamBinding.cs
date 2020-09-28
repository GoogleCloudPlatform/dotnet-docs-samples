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

// [START storage_add_bucket_conditional_iam_binding]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class AddBucketConditionalIamBindingSample
{
    /// <summary>
    /// Adds a conditional Iam policy to a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="role">The role that members may assume.</param>
    /// <param name="member">The identifier of the member who may assume the provided role.</param>
    /// <param name="title">Title for the expression.</param>
    /// <param name="description">Description of the expression.</param>
    /// <param name="expression">Describes the conditions that need to be met for the policy to be applied.
    /// It's represented as a string using Common Expression Language syntax.</param>
    public Policy AddBucketConditionalIamBinding(
        string bucketName = "your-unique-bucket-name",
        string role = "roles/storage.objectViewer",
        string member = "serviceAccount:dev@iam.gserviceaccount.com",
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
        Policy.BindingsData bindingToAdd = new Policy.BindingsData
        {
            Role = role,
            Members = new List<string> { member },
            Condition = new Expr
            {
                Title = title,
                Description = description,
                Expression = expression
            }
        };

        policy.Bindings.Add(bindingToAdd);

        var bucketIamPolicy = storage.SetBucketIamPolicy(bucketName, policy);
        Console.WriteLine($"Added {member} with role {role} " + $"to {bucketName}");
        return bucketIamPolicy;
    }
}
// [END storage_add_bucket_conditional_iam_binding]
