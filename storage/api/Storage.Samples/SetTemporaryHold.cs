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

// [START storage_set_temporary_hold]

using Google.Cloud.Storage.V1;

public class SetTemporaryHoldSample
{
    public void SetTemporaryHold(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name")
    {
        var storage = StorageClient.Create();
        var storageObject = storage.GetObject(bucketName, objectName);
        storageObject.TemporaryHold = true;
        storage.UpdateObject(storageObject);
        System.Console.WriteLine($"Temporary hold was set for {objectName}.");
    }
}
// [END storage_set_temporary_hold]
