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
using Google.Events.Protobuf.Firebase.Auth.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Firebase.Tests;

public class FirebaseAuthTest : FunctionTestBase<FirebaseAuth.Function>
{
    [Fact]
    public async Task LoggingForMinimalEvent()
    {
        var data = new AuthEventData { Uid = "my-uid" };
        await ExecuteCloudEventRequestAsync(AuthEventData.CreatedCloudEventType, data);

        var entry = Assert.Single(GetFunctionLogEntries());
        Assert.Equal("Function triggered by change to user: my-uid", entry.Message);
    }


    [Fact]
    public async Task CreatedLoggedWhenPresent()
    {
        var created = new DateTimeOffset(2020, 8, 26, 17, 29, 30, TimeSpan.Zero);
        var data = new AuthEventData
        {
            Uid = "my-uid",
            Metadata = new UserMetadata { CreateTime = created.ToTimestamp() }
        };
        await ExecuteCloudEventRequestAsync(AuthEventData.CreatedCloudEventType, data);

        var entries = GetFunctionLogEntries();
        Assert.Equal(2, entries.Count);
        // The first entry is the UID
        Assert.Equal("User created at: 2020-08-26T17:29:30", entries[1].Message);
    }

    [Fact]
    public async Task EmailLoggedWhenPresent()
    {
        var data = new AuthEventData { Uid = "my-uid", Email = "noone@nowhere.com" };
        await ExecuteCloudEventRequestAsync(AuthEventData.CreatedCloudEventType, data);

        var entries = GetFunctionLogEntries();
        Assert.Equal(2, entries.Count);
        // The first entry is the UID
        Assert.Equal("Email: noone@nowhere.com", entries[1].Message);
    }
}
