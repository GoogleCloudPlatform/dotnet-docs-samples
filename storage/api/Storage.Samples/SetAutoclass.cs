// Copyright 2022 Google Inc.
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

// [START storage_set_autoclass]

using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using static Google.Apis.Storage.v1.Data.Bucket;
using System;

public class SetAutoclassSample
{
    public Bucket SetAutoclass(string bucketName)
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        bucket.Autoclass = new AutoclassData { Enabled = true, TerminalStorageClass = "ARCHIVE" };
        storage.PatchBucket(bucket);
        Console.WriteLine($"Autoclass enabled is set to {bucket.Autoclass.Enabled} for {bucketName} at {bucket.Autoclass.ToggleTimeDateTimeOffset}.");
        Console.WriteLine($"Autoclass terminal storage class is {bucket.Autoclass.TerminalStorageClass}.");
        return bucket;
    }
}
// [END storage_set_autoclass]
