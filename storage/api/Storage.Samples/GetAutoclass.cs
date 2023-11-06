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

// [START storage_get_autoclass]

using Google.Cloud.Storage.V1;
using System;
using static Google.Apis.Storage.v1.Data.Bucket;

public class GetAutoclassSample
{
    public AutoclassData GetAutoclass(string bucketName)
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        Console.WriteLine($"Autoclass enabled is set to {bucket.Autoclass.Enabled} for {bucketName} at {bucket.Autoclass.ToggleTimeDateTimeOffset}.");
        Console.WriteLine($"Autoclass terminal storage class is set to {bucket.Autoclass.TerminalStorageClass} for {bucketName} at {bucket.Autoclass.TerminalStorageClassUpdateTimeDateTimeOffset}.");
        return bucket.Autoclass;
    }
}
// [END storage_get_autoclass]
