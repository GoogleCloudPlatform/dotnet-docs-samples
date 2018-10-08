// Copyright (c) 2018 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START monitoring_alert_delete_channel]

using Google.Cloud.Monitoring.V3;
using System;

partial class AlertSnippets
{
    public void DeleteNotificationChannel(
        string channelName = "projects/your-project-id/notificationChannels/123")
    {
        var client = NotificationChannelServiceClient.Create();
        client.DeleteNotificationChannel(
            name: NotificationChannelName.Parse(channelName),
            force: true);
        Console.WriteLine("Deleted {0}.", channelName);
    }
}

// [END monitoring_alert_delete_channel]