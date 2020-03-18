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

// [START scc_create_notification_config]
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
// [END scc_create_notification_config]

/** Create NotificationConfig Snippet. */
public class CreateNotificationConfigSnippets
{
    private CreateNotificationConfigSnippets() {}

    // [START scc_create_notification_config]
    public static NotificationConfig createNotificationConfig(
        String organizationId, String notificationConfigId, String projectId, String topicName)
    {
        // String organizationId = "{your-org-id}";
        // String notificationConfigId = {"your-unique-id"};
        // String projectId = "{your-project}"";
        // String topicName = "{your-topic}";

        String orgName = String.Format("organizations/{0}", organizationId);
        // Ensure this ServiceAccount has the "pubsub.topics.setIamPolicy" permission on the topic.
        String pubsubTopic = String.Format("projects/{0}/topics/{1}", projectId, topicName);

        SecurityCenterClient client = SecurityCenterClient.Create();
        CreateNotificationConfigRequest request = new CreateNotificationConfigRequest
        {
            Parent=orgName,
            ConfigId=notificationConfigId,
            NotificationConfig=new NotificationConfig
            {
                Description="Java notification config",
                PubsubTopic=pubsubTopic,
                StreamingConfig=
                    new NotificationConfig.Types.StreamingConfig{Filter="state = \"ACTIVE\""}
            }
        };

        NotificationConfig response = client.CreateNotificationConfig(request);
        Console.WriteLine(String.Format("Notification config was created: {0}", response));
        return response;
    }
    // [END scc_create_notification_config]
}
