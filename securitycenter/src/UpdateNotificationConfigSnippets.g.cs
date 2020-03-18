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

namespace Snippets
{
    // [START scc_update_notification_config]
    using Google.Cloud.SecurityCenter.V1;
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    // [END scc_update_notification_config]

/** Snippets for UpdateNotificationConfig. */
    public class UpdateNotificationConfigSnippets
    {
    private UpdateNotificationConfigSnippets() {}

        // [START scc_update_notification_config]
        public static NotificationConfig updateNotificationConfig(
            String organizationId, String notificationConfigId, String projectId, String topicName)
        {
            // String organizationId = "{your-org-id}";
            // String notificationConfigId = "{your-config-id}";
            // String projectId = "{your-project}";
            // String topicName = "{your-topic}";

            String notificationConfigName =
                String.Format(
                    "organizations/{0}/notificationConfigs/{1}", organizationId, notificationConfigId);

            // Ensure this ServiceAccount has the "pubsub.topics.setIamPolicy" permission on the topic.
            String pubsubTopic = String.Format("projects/{0}/topics/{1}", projectId, topicName);

            NotificationConfig configToUpdate = new NotificationConfig
            {
                Name=notificationConfigName,
                Description="updated description",
                PubsubTopic=pubsubTopic
            };

            FieldMask fieldMask = new FieldMask{Paths={"description", "pubsub_topic"}};
            SecurityCenterClient client = SecurityCenterClient.Create();
            NotificationConfig updatedConfig = client.UpdateNotificationConfig(configToUpdate, fieldMask);

            Console.WriteLine(String.Format("Notification config updated: {0}", updatedConfig));
            return updatedConfig;
        }
    }
    // [END scc_update_notification_config]
}