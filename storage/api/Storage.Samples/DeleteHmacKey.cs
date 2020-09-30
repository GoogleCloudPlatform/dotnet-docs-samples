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

// [START storage_delete_hmac_key]

using Google.Cloud.Storage.V1;
using System;

public class DeleteHmacKeySample
{
    public void DeleteHmacKey(
        string projectId = "your-project-id",
        string accessId = "your-access-id")
    {
        var storage = StorageClient.Create();
        storage.DeleteHmacKey(projectId, accessId);

        Console.WriteLine($"Key {accessId} was deleted.");
    }
}
// [END storage_delete_hmac_key]
