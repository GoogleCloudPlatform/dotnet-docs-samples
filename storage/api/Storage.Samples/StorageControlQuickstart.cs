// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START storage_control_quickstart_sample]
using Google.Cloud.Storage.Control.V2;
using System;

public class StorageControlQuickstartSample
{
    public StorageLayout StorageControlQuickstart(string bucketName = "your-unique-bucket-name")
    {
        StorageControlClient storageControl = StorageControlClient.Create();

        // Using "_" for projectId means global bucket namespace
        var layoutName = new StorageLayoutName("_", bucketName);
        StorageLayout response = storageControl.GetStorageLayout(layoutName);

        Console.WriteLine($"Bucket {bucketName} has location type {response.LocationType}");
        return response;
    }
}
// [END storage_control_quickstart_sample]
