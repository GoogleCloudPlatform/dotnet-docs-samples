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
using Google.Cloud.Functions.Invoker.Testing;
using Google.Events;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.Tests
{
    public abstract class HelloPubSubTestBase<TFunction> : FunctionTestBase<TFunction>
    {
        [Fact]
        public async Task MessageWithTextData()
        {
            var cloudEvent = new CloudEvent(MessagePublishedData.MessagePublishedCloudEventType, new Uri("//pubsub.googleapis.com"));
            var data = new MessagePublishedData
            {
                 Message = new PubsubMessage { TextData = "PubSub user" }
            };
            CloudEventConverters.PopulateCloudEvent(cloudEvent, data);

            await ExecuteCloudEventRequestAsync(cloudEvent);
            var logEntry = Assert.Single(GetFunctionLogEntries());
            Assert.Equal("Hello PubSub user", logEntry.Message);
            Assert.Equal(LogLevel.Information, logEntry.Level);
        }

        [Fact]
        public async Task MessageWithoutTextData()
        {
            var cloudEvent = new CloudEvent(MessagePublishedData.MessagePublishedCloudEventType, new Uri("//pubsub.googleapis.com"));
            var data = new MessagePublishedData
            {
                Message = new PubsubMessage { Attributes = { { "key", "value" } } }
            };
            CloudEventConverters.PopulateCloudEvent(cloudEvent, data);

            await ExecuteCloudEventRequestAsync(cloudEvent);
            var logEntry = Assert.Single(GetFunctionLogEntries());
            Assert.Equal("Hello world", logEntry.Message);
            Assert.Equal(LogLevel.Information, logEntry.Level);
        }
    }

    // C# test
    public class HelloPubSubTest : HelloPubSubTestBase<HelloPubSub.Function>
    {
    }

    // VB test
    public class HelloPubSubVbTest : HelloPubSubTestBase<HelloPubSubVb.CloudFunction>
    {
    }

    // F# test
    public class HelloPubSubFSharpTest : HelloPubSubTestBase<HelloPubSubFSharp.Function>
    {
    }
}
