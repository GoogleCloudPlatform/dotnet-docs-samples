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

// [START storage_enable_bucket_lifecycle_management]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class EnableBucketLifecycleManagementSample
{
    public Bucket EnableBucketLifecycleManagement(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        bucket.Lifecycle = new Bucket.LifecycleData
        {
            Rule = new List<Bucket.LifecycleData.RuleData>
            {
                new Bucket.LifecycleData.RuleData
                {
                    Condition = new Bucket.LifecycleData.RuleData.ConditionData { Age = 100 },
                    Action = new Bucket.LifecycleData.RuleData.ActionData { Type = "Delete" }
                }
            }
        };
        bucket = storage.UpdateBucket(bucket);

        Console.WriteLine($"Lifecycle management is enabled for bucket {bucketName} and the rules are:");
        foreach (var rule in bucket.Lifecycle.Rule)
        {
            Console.WriteLine("Action:");
            Console.WriteLine($"Type: {rule.Action.Type}");
            Console.WriteLine($"Storage Class: {rule.Action.StorageClass}");

            Console.WriteLine("Condition:");
            Console.WriteLine($"Age: \t{rule.Condition.Age}");
            Console.WriteLine($"Created Before: \t{rule.Condition.CreatedBefore}");
            Console.WriteLine($"Time Before: \t{rule.Condition.CustomTimeBefore}");
            Console.WriteLine($"Days Since Custom Time: \t{rule.Condition.DaysSinceCustomTime}");
            Console.WriteLine($"Days Since Non-current Time: \t{rule.Condition.DaysSinceNoncurrentTime}");
            Console.WriteLine($"IsLive: \t{rule.Condition.IsLive}");
            Console.WriteLine($"Pattern: \t{rule.Condition.MatchesPattern}");
            Console.WriteLine($"Storage Class: \t{rule.Condition.MatchesStorageClass}");
            Console.WriteLine($"Noncurrent Time Before: \t{rule.Condition.NoncurrentTimeBefore}");
            Console.WriteLine($"Newer Versions: \t{rule.Condition.NumNewerVersions}");
        }
        return bucket;
    }
}
// [END storage_enable_bucket_lifecycle_management]
