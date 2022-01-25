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

// [START storage_delete_bucket_notification]

using System;
using Google.Cloud.Storage.V1;

public class DeletePubSubNotificationSample
{
    public void DeletePubSubNotification(
        string bucketName = "your-unique-bucket-name",
        string notificationId = "notificationId")
    {
        StorageClient storage = StorageClient.Create();
        storage.DeleteNotification(bucketName, notificationId);

        Console.WriteLine("Successfully deleted notification with ID " + notificationId + " for bucket " + bucketName);
    }
}

// [END storage_delete_bucket_notification]
