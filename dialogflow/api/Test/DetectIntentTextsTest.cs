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

using Xunit;

namespace GoogleCloudSamples
{
    public class DetectIntentTextsTest : DialogflowTest
    {
        [Fact]
        void TestDetectIntentFromTexts()
        {
            var texts = new[] { "hello", "book a meeting room", "Mountain View" };
            var textsArgument = string.Join(',', texts);

            RunWithSessionId("detect-intent:texts", textsArgument);
            Assert.Equal(0, ExitCode);

            Assert.Contains("Query text:", Stdout);
            Assert.Contains("Intent confidence:", Stdout);
            Assert.Contains("Fulfillment text:", Stdout);
        }
    }
}
