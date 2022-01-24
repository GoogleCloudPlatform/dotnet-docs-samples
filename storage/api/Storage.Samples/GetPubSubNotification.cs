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

// [START storage_print_pubsub_bucket_notification]

using System;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;

public class GetPubSubNotificationSample
{
    public Notification GetPubSubNotification(
     string bucketName = "your-unique-bucket-name",
     string notificationId = "notification-Id")
    {
        var storage = StorageClient.Create();
        var notification = storage.GetNotification(bucketName, notificationId);

        Console.WriteLine("ID: " + notification.Id);
        Console.WriteLine("Topic: " + notification.Topic);
        Console.WriteLine("EventTypes: " + notification.EventTypes);
        Console.WriteLine("CustomAttributes: " + notification.CustomAttributes);
        Console.WriteLine("PayloadFormat: " + notification.PayloadFormat);
        Console.WriteLine("ObjectNamePrefix: " + notification.ObjectNamePrefix);
        Console.WriteLine("ETag: " + notification.ETag);
        Console.WriteLine("SelfLink: " + notification.SelfLink);
        Console.WriteLine("Kind: " + notification.Kind);

        return notification;
    }
}

// [END storage_print_pubsub_bucket_notification]
