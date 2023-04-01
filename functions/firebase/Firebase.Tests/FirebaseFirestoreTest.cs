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
using Google.Events.Protobuf.Cloud.Firestore.V1;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Firebase.Tests;

public class FirebaseFirestoreTest : FunctionTestBase<FirebaseFirestore.Function>
{
    [Fact]
    public async Task FullLogging()
    {
        string documentName = "projects/my-project/databases/(default)/documents/games/xyz";
        var data = new DocumentEventData
        {
            OldValue = new Document
            {
                Name = documentName,
                Fields =
                {
                    { "highScore", new Value { IntegerValue = 10 } },
                    { "name", new Value { StringValue = "Function Tester" } }
                }
            },
            Value = new Document
            {
                Name = documentName,
                Fields =
                {
                    { "highScore", new Value { IntegerValue = 20 } },
                    { "name", new Value { StringValue = "Function Tester" } }
                }
            }
        };
        await ExecuteCloudEventRequestAsync(
            DocumentEventData.UpdatedCloudEventType,
            data,
            new Uri("//firestore.googleapis.com/projects/my-project"),
            "documents/games/xyz");

        var logEntries = GetFunctionLogEntries();

        var expectedMessages = new[]
        {
            "Function triggered by event on documents/games/xyz",
            "Event type: google.cloud.firestore.document.v1.updated",
            "Old value: highScore: 10, name: Function Tester",
            "New value: highScore: 20, name: Function Tester"
        };
        var actualMessages = GetFunctionLogEntries().Select(entry => entry.Message).ToArray();

        Assert.Equal(expectedMessages, actualMessages);
    }

    [Fact]
    public async Task MissingFieldsIgnored()
    {
        // There really would normally be Value or OldValue, but it's simpler to test
        // a completely empty payload.
        var data = new DocumentEventData();
        await ExecuteCloudEventRequestAsync(
            DocumentEventData.DeletedCloudEventType,
            data,
            new Uri("//firestore.googleapis.com/projects/my-project"),
            "documents/games/xyz");

        // We still have the trigger and type log entries, but not the values.
        Assert.Equal(2, GetFunctionLogEntries().Count);
    }
}
