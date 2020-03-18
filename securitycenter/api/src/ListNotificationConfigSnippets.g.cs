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
    // [START scc_list_notification_configs]
    using Google.Cloud.SecurityCenter.V1;
    using Google.Api.Gax.ResourceNames;
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using System;
    using System.Collections;
    using System.Collections.Immutable;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    // [END scc_list_notification_configs]

    /** Snippets for how to ListNotificationConfig. */
    public class ListNotificationConfigSnippets
    {
        private ListNotificationConfigSnippets() {}

        public static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        // [START scc_list_notification_configs]
        public static ImmutableList<NotificationConfig> listNotificationConfigs(String organizationId)
        {
            // String organizationId = "{your-org-id}";
            OrganizationName orgName = new OrganizationName(organizationId);
            SecurityCenterClient client = SecurityCenterClient.Create();
            Google.Api.Gax.PagedEnumerable<Google.Cloud.SecurityCenter.V1.ListNotificationConfigsResponse, Google.Cloud.SecurityCenter.V1.NotificationConfig> response = client.ListNotificationConfigs(orgName);
            ImmutableList<NotificationConfig> notificationConfigs = response.ReadPage(50).ToImmutableList();
            
            // Print Notification Configuration names.
            notificationConfigs.ForEach(config => Console.WriteLine(config.NotificationConfigName));
            return notificationConfigs;
            }
    }
        // [END scc_list_notification_configs]
}