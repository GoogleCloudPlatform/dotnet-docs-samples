﻿// Copyright 2020 Google Inc.
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

namespace Storage
{
    public class StorageViewBucketIamMembers
    {
        // [START storage_view_bucket_iam_members]
        public static void ViewBucketIamMembers(string bucketName)
        {
            var storage = StorageClient.Create();
            var policy = storage.GetBucketIamPolicy(bucketName);
            foreach (var binding in policy.Bindings)
            {
                Console.WriteLine($"  Role: {binding.Role}");
                Console.WriteLine("  Members:");
                foreach (var member in binding.Members)
                {
                    Console.WriteLine($"    {member}");
                }
            }
        }
        // [END storage_view_bucket_iam_members]
    }
}
