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
using Google.Events.Protobuf.Firebase.Database.V1;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;
using Xunit;

namespace Firebase.Tests;

public class FirebaseRtdbTest : FunctionTestBase<FirebaseRtdb.Function>
{
    [Fact]
    public async Task SubjectAndDeltaAreLogged()
    {
        var data = new ReferenceEventData
        {
            Data = Value.ForNumber(5),
            Delta = Value.ForNumber(10)
        };
        await ExecuteCloudEventRequestAsync(ReferenceEventData.UpdatedCloudEventType, data, subject: "refs/sample_ref/score");

        var logEntries = GetFunctionLogEntries();
        Assert.Contains(logEntries, entry => entry.Message == "Function triggered by change to refs/sample_ref/score");
        Assert.Contains(logEntries, entry => entry.Message == "Delta: 10");
    }
}
