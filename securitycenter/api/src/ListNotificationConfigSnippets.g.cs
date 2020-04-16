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

// [START scc_list_notification_configs]
using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecurityCenter.V1;
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

/** Snippets for how to ListNotificationConfig. */
public class ListNotificationConfigSnippets
{
    public static ImmutableList<NotificationConfig> ListNotificationConfigs(string organizationId)
    {
        OrganizationName orgName = new OrganizationName(organizationId);
        SecurityCenterClient client = SecurityCenterClient.Create();
        ImmutableList<NotificationConfig> notificationConfigs = client.ListNotificationConfigs(orgName).ReadPage(50).ToImmutableList();

        // Print Notification Configuration names.
        notificationConfigs.ForEach(config => Console.WriteLine(config.NotificationConfigName));
        return notificationConfigs;
    }
}
// [END scc_list_notification_configs]
