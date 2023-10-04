// Copyright 2020 Google Inc.
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

using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class RedactImageTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public RedactImageTests(DlpTestFixture fixture) => _fixture = fixture;
        [Fact]
        public void TestRedactImage()
        {
            var input = Path.Combine(_fixture.ResourcePath, "test_dlp_redact_image.png");
            var output = Path.Combine(_fixture.ResourcePath, "dlp_redacted_image.png");
            var redacted = RedactImage.Redact(_fixture.ProjectId, input, output);
            Assert.True(File.Exists(output));
            Assert.Contains("223-456-7890", redacted.ExtractedText);
        }
    }
}
