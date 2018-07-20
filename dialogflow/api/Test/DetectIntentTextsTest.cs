// Copyright(c) 2018 Google Inc.
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

using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class DetectIntentTextsTest : DialogflowTest
    {
        [Fact(Skip = "b/110877421")]
        void TestDetectIntentFromTexts()
        {
            var texts = new[] { "hello", "book a meeting room", "Mountain View" };
            var textsArgument = string.Join(',', texts);

            RunWithSessionId("detect-intent:texts", textsArgument);
            Assert.Equal(0, ExitCode);

            // "hello"
            Assert.Contains("Query text: hello", Stdout);
            Assert.Contains("Intent detected: Default Welcome Intent", Stdout);
            Assert.Contains("I'm your room booking bot", Stdout);

            // "book a meeting room"
            Assert.Contains("Query text: book a meeting room", Stdout);
            Assert.Contains("Intent detected: room.reservation", Stdout);
            Assert.Contains("Where would you like to reserve a room?", Stdout);

            // "Mountain View"
            Assert.Contains("Query text: Mountain View", Stdout);
            Assert.Contains("Intent detected: room.reservation", Stdout);
            Assert.Contains("What date?", Stdout);

            // Don't test exact values (dynamic).
            // Assert these lines are printed.
            Assert.Contains("Intent confidence:", Stdout);
            Assert.Contains("Fulfillment text:", Stdout);
        }
    }
}
