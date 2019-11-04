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

using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class GetGlossaryTests : IDisposable
    {
        private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        private readonly string _glossaryId;
        private readonly string _glossaryInputUri = "gs://cloud-samples-data/translation/glossary_ja.csv";

        private readonly CommandLineRunner _sample = new CommandLineRunner()
        {
            VoidMain = TranslateV3Samples.Main
        };

        // Setup
        public GetGlossaryTests()
        {
            _glossaryId = "translate-v3" + TestUtil.RandomName();
            CreateGlossary.CreateGlossarySample(_projectId, _glossaryId, _glossaryInputUri);
        }

        // TearDown
        public void Dispose()
        {
            DeleteGlossary.DeleteGlossarySample(_projectId, _glossaryId);
        }

        [Fact]
        public void GetGlossaryTest()
        {
            var output = _sample.Run("getGlossary",
                "--project_id=" + _projectId,
                "--glossary_id=" + _glossaryId);
            Assert.Contains("gs://cloud-samples-data/translation/glossary_ja.csv", output.Stdout);
        }
    }
}



