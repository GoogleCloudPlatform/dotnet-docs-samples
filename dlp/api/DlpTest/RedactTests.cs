// Copyright 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Testing redact samples for DLP
    /// </summary>
    public class RedactTests : IClassFixture<DlpTestFixture>
    {
        private readonly DlpTestFixture _testSettings;

        public RedactTests(DlpTestFixture fixture)
        {
            _testSettings = fixture;
        }

        [Fact]
        public void TestRedactImage()
        {
            string inPath = Path.Combine("resources", "test.png");
            string outPath = "redacted.png";

            ConsoleOutput output = _testSettings.CommandLineRunner.Run(
                "redactImage",
                "--projectId", _testSettings.ProjectId,
                "--imageFromPath", inPath,
                "--imageToPath", outPath);
            output.AssertSucceeded();

            var outFile = new FileInfo(outPath);

            Assert.True(File.Exists(outPath), "Redacted image exists");
            Assert.True(outFile.Length > 0, "Redacted image written");
        }
    }
}
