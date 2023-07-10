// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START securitycenter_get_notification_config]

using Google.Cloud.SecurityCenter.V1;
using System;

/// <summary>Snippet for GetNotificationConfig</summary>
public class GetNotificationConfigSnippets
{
    public static NotificationConfig GetNotificationConfig(string organizationId, string configId)
    {
        SecurityCenterClient client = SecurityCenterClient.Create();
        // You can also use 'projectId' or 'folderId' instead of the 'organizationId'.
        NotificationConfigName notificationConfigName = new NotificationConfigName(organizationId, configId);

        NotificationConfig response = client.GetNotificationConfig(notificationConfigName);
        Console.WriteLine($"Notification config: {response}");
        return response;
    }
}
// [END securitycenter_get_notification_config]
