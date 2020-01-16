// Copyright 2019 Google LLC
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

using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(TranslateFixture))]
    public class ListGlossaryTests
    {
        private readonly TranslateFixture _fixture;

        public ListGlossaryTests(TranslateFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ListGlossariesTest()
        {
            var output = _fixture.SampleRunner.Run("listGlossaries",
                "--project_id=" + _fixture.ProjectId);
            Assert.Contains("gs://cloud-samples-data/translation/glossary_ja.csv", output.Stdout);
        }
    }
}


