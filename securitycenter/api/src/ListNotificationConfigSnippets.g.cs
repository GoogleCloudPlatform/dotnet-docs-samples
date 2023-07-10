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

// [START securitycenter_list_notification_configs]

using Google.Api.Gax.ResourceNames;
using Google.Api.Gax;
using Google.Cloud.SecurityCenter.V1;
using System;

/// <summary>Snippet for ListNotificationConfig</summary>
public class ListNotificationConfigSnippets
{
    public static PagedEnumerable<ListNotificationConfigsResponse, NotificationConfig> ListNotificationConfigs(string organizationId)
    {
        // You can also use 'projectId' or 'folderId' instead of the 'organizationId'.
        //      ProjectName projectName = new ProjectName(projectId);
        //      FolderName folderName = new FolderName(folderId);
        OrganizationName orgName = new OrganizationName(organizationId);
        SecurityCenterClient client = SecurityCenterClient.Create();
        PagedEnumerable<ListNotificationConfigsResponse, NotificationConfig> notificationConfigs = client.ListNotificationConfigs(orgName);

        // Print Notification Configuration names.
        foreach (var config in notificationConfigs)
        {
            Console.WriteLine(config.NotificationConfigName);
        }
        return notificationConfigs;
    }
}
// [END securitycenter_list_notification_configs]
