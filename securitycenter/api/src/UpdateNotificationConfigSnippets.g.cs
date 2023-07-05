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

// [START securitycenter_update_notification_config]

using Google.Cloud.SecurityCenter.V1;
using static Google.Cloud.SecurityCenter.V1.NotificationConfig.Types;
using Google.Protobuf.WellKnownTypes;
using System;

/// <summary>Snippet for UpdateNotificationConfig</summary>
public class UpdateNotificationConfigSnippets
{
    public static NotificationConfig UpdateNotificationConfig(
        string organizationId, string notificationConfigId, string projectId, string topicName)
    {
        // You can also use 'projectId' or 'folderId' instead of the 'organizationId'.
        NotificationConfigName notificationConfigName = new NotificationConfigName(organizationId, notificationConfigId);
        TopicName pubsubTopic = new TopicName(projectId, topicName);

        NotificationConfig configToUpdate = new NotificationConfig
        {
            NotificationConfigName = notificationConfigName,
            Description = "updated description",
            PubsubTopicAsTopicName = pubsubTopic,
            StreamingConfig = new StreamingConfig { Filter = "state = \"INACTIVE\"" }
        };

        FieldMask fieldMask = new FieldMask { Paths = { "description", "pubsub_topic", "streaming_config.filter" } };
        SecurityCenterClient client = SecurityCenterClient.Create();
        NotificationConfig updatedConfig = client.UpdateNotificationConfig(configToUpdate, fieldMask);

        Console.WriteLine($"Notification config updated: {updatedConfig}");
        return updatedConfig;
    }
}
// [END securitycenter_update_notification_config]
