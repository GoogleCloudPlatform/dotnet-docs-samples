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

// [START monitoring_alert_enable_channel]

using Google.Cloud.Monitoring.V3;
using Google.Protobuf.WellKnownTypes;
using System;

partial class AlertSnippets
{
    public NotificationChannel EnableNotificationChannel(
        string channelName = "projects/your-project-id/notificationChannels/123")
    {
        var client = NotificationChannelServiceClient.Create();
        NotificationChannel channel = new NotificationChannel();
        channel.Enabled = true;
        channel.Name = channelName;
        var fieldMask = new FieldMask { Paths = { "enabled" } };
        channel = client.UpdateNotificationChannel(
            updateMask: fieldMask,
            notificationChannel: channel);
        Console.WriteLine("Enabled {0}.", channel.Name);
        return channel;
    }
}

// [END monitoring_alert_enable_channel]