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
    public class DeidentifyWithDateShiftTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public DeidentifyWithDateShiftTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestDeidentifyWithDateShift()
        {
            var inputFilePath = Path.Combine(Fixture.ResourcePath, "dates-input.csv");
            Assert.True(File.Exists(inputFilePath));
            var result = DeidentifyWithDateShift.Deidentify(Fixture.ProjectId, inputFilePath, 50, 50, "birth_date,register_date", "name", Fixture.KeyName, Fixture.WrappedKey);
            Assert.NotNull(result);
            Assert.Contains(result.Overview.TransformationSummaries, ts => ts.Field.Name == "birth_date");
        }
    }
}
