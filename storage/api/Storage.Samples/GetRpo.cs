// Copyright 2021 Google Inc.
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

// [START storage_get_rpo]

using System;
using Google.Cloud.Storage.V1;

public class GetRpoSample
{
    public string GetRpo(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var rpo = storage.GetBucket(bucketName).Rpo;
        Console.WriteLine($"The RPO Setting of bucket {bucketName} is {rpo}.");

        return rpo;
    }
}

// [END storage_get_rpo]
