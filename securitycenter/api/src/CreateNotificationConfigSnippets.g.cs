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

// [START securitycenter_create_notification_config]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecurityCenter.V1;
using System;

///<summary> Create NotificationConfig Snippet. </summary>
public class CreateNotificationConfigSnippets
{
    public static NotificationConfig CreateNotificationConfig(
        string organizationId, string notificationConfigId, string projectId, string topicName)
    {
        OrganizationName orgName = new OrganizationName(organizationId);
        TopicName pubsubTopic = new TopicName(projectId, topicName);

        SecurityCenterClient client = SecurityCenterClient.Create();
        CreateNotificationConfigRequest request = new CreateNotificationConfigRequest
        {
            ParentAsOrganizationName = orgName,
            ConfigId = notificationConfigId,
            NotificationConfig = new NotificationConfig
            {
                Description = ".Net notification config",
                PubsubTopicAsTopicName = pubsubTopic,
                StreamingConfig = new NotificationConfig.Types.StreamingConfig { Filter = "state = \"ACTIVE\"" }
            }
        };

        NotificationConfig response = client.CreateNotificationConfig(request);
        Console.WriteLine($"Notification config was created: {response}");
        return response;
    }
}
// [END securitycenter_create_notification_config]
