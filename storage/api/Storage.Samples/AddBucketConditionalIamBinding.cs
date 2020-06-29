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
    public Policy AddBucketConditionalIamBinding(string bucketName, string role,
        string member, string title, string description, string expression)
    {
        var storage = StorageClient.Create();
        var policy = storage.GetBucketIamPolicy(bucketName, new GetBucketIamPolicyOptions()
        {
            RequestedPolicyVersion = 3
        });

        policy.Version = 3;

        Policy.BindingsData bindingToAdd = new Policy.BindingsData()
        {
            Role = role,
            Members = new List<string>() { member },
            Condition = new Expr()
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
