// Copyright(c) 2020 Google Inc.
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
using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class DetectIntentStreamsTest : DialogflowTest
    {
        [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/947")]
        void TestDetectIntentFromStream()
        {
            var _audioWavPath = Path.Combine("resources", "book_a_room.wav");

            RunWithSessionId("detect-intent:streams", _audioWavPath);
            Assert.Equal(0, ExitCode);

            Assert.Contains("book", Stdout);
            Assert.Contains("Intent detected:", Stdout);
        }
    }
}
