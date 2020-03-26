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

// [START scc_get_notification_config]
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
// [END scc_get_notification_config]

/** Get NotificationConfig Snippet. */
public class GetNotificationConfigSnippets
{
    private GetNotificationConfigSnippets(){}

    /// <summary>Snippet for GetNotificationConfig</summary>
    // [START scc_get_notification_config]
    public static NotificationConfig GetNotificationConfig(
        string organizationId, string configId)
    {
        SecurityCenterClient client = SecurityCenterClient.Create();

        // ConfigName is in the format "organizations/{org_id}/notificationConfigs/{config_id}";
        GetNotificationConfigRequest request = new GetNotificationConfigRequest{
            NotificationConfigName = new NotificationConfigName(organizationId, configId)};
        NotificationConfig response = client.GetNotificationConfig(request);
        Console.WriteLine($"Notification config: {response}");
        return response;
    }
    // [END scc_get_notification_config]
}
