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

namespace Storage
{
    public class SetTemporarydHold
    {
        public static void SetObjectTemporaryHold(string bucketName,
            string objectName)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(bucketName, objectName);
            storageObject.TemporaryHold = true;
            storageObject = storage.UpdateObject(storageObject, new UpdateObjectOptions()
            {
                // Use IfMetagenerationMatch to avoid race conditions.
                IfMetagenerationMatch = storageObject.Metageneration,
            });
        }
    }
}
// [END storage_set_temporary_hold]
