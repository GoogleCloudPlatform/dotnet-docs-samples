// Copyright 2023 Google Inc.
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
// limitations under the License

using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectImagesForSensitiveDataWithListedInfoTypesTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public InspectImagesForSensitiveDataWithListedInfoTypesTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestInspectImage()
        {
            var input = Path.Combine(_fixture.ResourcePath, "test_inspect_image_listed_infotypes.png");
            var result = InspectImageForSensitiveDataWithListedInfoTypes.InspectImage(_fixture.ProjectId, input);
            Assert.Equal(2, result.Result.Findings.Count);
            Assert.Contains(result.Result.Findings, f => f.InfoType.Name == "PHONE_NUMBER");
            Assert.Contains(result.Result.Findings, f => f.Quote == "223-456-7890"); // 223-456-7890 is phone number in the image.
        }
    }
}