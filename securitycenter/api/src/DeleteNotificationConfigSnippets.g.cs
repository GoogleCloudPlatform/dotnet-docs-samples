/*
 * Copyright 2020 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START scc_delete_notification_config]

using Google.Cloud.SecurityCenter.V1;
using System;

/// <summary>Snippet for DeleteNotificationConfig</summary>
public class DeleteNotificationConfigSnippets
{
    public static bool DeleteNotificationConfig(string organizationId, string notificationConfigId)
    {
        NotificationConfigName notificationConfigName = new NotificationConfigName(organizationId, notificationConfigId);
        SecurityCenterClient client = SecurityCenterClient.Create();

        client.DeleteNotificationConfig(notificationConfigName);
        Console.WriteLine($"Deleted Notification config: {notificationConfigName}");
        return true;
    }
}
// [END scc_delete_notification_config]
