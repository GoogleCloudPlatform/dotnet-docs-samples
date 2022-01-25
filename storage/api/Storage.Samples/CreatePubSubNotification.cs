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

// [START storage_create_bucket_notifications]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class CreatePubSubNotificationSample
{
    public Notification CreatePubSubNotification(
        string bucketName = "your-unique-bucket-name",
        string topic = "my-topic")
    {
        StorageClient storage = StorageClient.Create();
        Notification notification = new Notification
        {
            Topic = topic,
            PayloadFormat = "JSON_API_V1"
        };

        Notification createdNotification = storage.CreateNotification(bucketName, notification);
        Console.WriteLine("Notification subscription created with ID: " + createdNotification.Id + " for bucket name " + bucketName);
        return createdNotification;
    }
}

// [END storage_create_bucket_notifications]
