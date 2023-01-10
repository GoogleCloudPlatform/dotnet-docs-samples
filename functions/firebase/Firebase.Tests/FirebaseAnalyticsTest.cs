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

using Google.Cloud.Functions.Testing;
using Google.Events.Protobuf.Firebase.Analytics.V1;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Firebase.Tests;

public class FirebaseAnalyticsTest : FunctionTestBase<FirebaseAnalytics.Function>
{
    [Fact]
    public async Task LoggingForEvent()
    {
        var data = new AnalyticsLogData
        {
            EventDim =
            {
                new EventDimensions
                {
                    Name = "my-event",
                    TimestampMicros = 1599818774000000L
                }
            },
            UserDim = new UserDimensions
            {
                DeviceInfo = new DeviceInfo { DeviceModel = "Pixel" },
                GeoInfo = new GeoInfo { City = "London", Country = "UK" }
            }
        };
        await ExecuteCloudEventRequestAsync(AnalyticsLogData.WrittenCloudEventType, data,
            new Uri("//firebaseanalytics.googleapis.com/projects/my-project/apps/my-app", UriKind.RelativeOrAbsolute));

        string[] expectedMessages =
        {
            "Event source: //firebaseanalytics.googleapis.com/projects/my-project/apps/my-app",
            "Event count: 1",
            "First event name: my-event",
            "First event timestamp: 2020-09-11 10:06:14Z",
            "Device model: Pixel",
            "Location: London, UK"
        };
        var actualMessages = GetFunctionLogEntries().Select(entry => entry.Message).ToArray();
        Assert.Equal(expectedMessages, actualMessages);
    }
}
