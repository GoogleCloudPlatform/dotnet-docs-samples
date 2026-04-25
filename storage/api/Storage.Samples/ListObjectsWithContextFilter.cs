// Copyright 2026 Google LLC
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

// [START storage_list_object_contexts]

using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;

public class ListObjectsWithContextFilterSample
{
    /// <summary>
    /// List objects in the specified bucket that match a context filter.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="filter">The metadata context filter (e.g. "contexts.\"KEY\":*", "-contexts.\"KEY\":*", "contexts.\"KEY\"=\"VALUE\"", "-contexts.\"KEY\"=\"VALUE\"").</param>
    public IEnumerable<Google.Apis.Storage.v1.Data.Object> ListObjectsWithContextFilter(string bucketName = "your-unique-bucket-name",
        string filter = "contexts.\"KEY\"=\"VALUE\"")
    {
        var storage = StorageClient.Create();
        var objects = storage.ListObjects(bucketName, prefix: null, new ListObjectsOptions { Filter = filter, Fields = "items(name,contexts)" }).ToList();
        Console.WriteLine($"The Names and Context Data for the Objects in the {bucketName} with Context Filter {filter} are as follows:");
        foreach (var obj in objects)
        {
            Console.WriteLine($"Object: {obj.Name}");
            Console.WriteLine($"Context Data for the Object {obj.Name} is as follows:");

            if (obj.Contexts?.Custom is { } customContexts)
            {
                foreach (var (key, contextPayload) in customContexts)
                {
                    Console.WriteLine($"Context Key: {key}, Context Value: {contextPayload.Value}");
                }
            }
        }
        return objects;
    }
}
// [END storage_list_object_contexts]
