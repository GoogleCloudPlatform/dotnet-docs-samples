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

// [START storage_get_default_event_based_hold]

using Google.Cloud.Storage.V1;
using System;

public class GetDefaultEventBasedHoldSample
{
    public bool GetDefaultEventBasedHold(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        bool defaultEventBasedHold = bucket.DefaultEventBasedHold ?? false;
        Console.WriteLine($"Default event-based hold: {defaultEventBasedHold}");
        return defaultEventBasedHold;
    }
}
// [END storage_get_default_event_based_hold]
