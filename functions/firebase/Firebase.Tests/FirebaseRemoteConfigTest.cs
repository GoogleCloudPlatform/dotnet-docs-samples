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

using CloudNative.CloudEvents;
using Google.Cloud.Functions.Testing;
using Google.Events;
using Google.Events.Protobuf.Firebase.RemoteConfig.V1;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Firebase.Tests;

public class FirebaseRemoteConfigTest : FunctionTestBase<FirebaseRemoteConfig.Function>
{
    [Fact]
    public async Task DetailsAreLogged()
    {
        var data = new RemoteConfigEventData
        {
            UpdateType = RemoteConfigUpdateType.IncrementalUpdate,
            UpdateOrigin = RemoteConfigUpdateOrigin.Console,
            VersionNumber = 12345L
        };

        await ExecuteCloudEventRequestAsync(RemoteConfigEventData.UpdatedCloudEventType, data);

        var expectedMessages = new[]
        {
            "Update type: IncrementalUpdate",
            "Update origin: Console",
            "Version number: 12345"
        };
        var actualMessages = GetFunctionLogEntries().Select(entry => entry.Message).ToArray();
        Assert.Equal(expectedMessages, actualMessages);
    }
}
