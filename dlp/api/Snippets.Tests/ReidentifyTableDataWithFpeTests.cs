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
// limitations under the License.

using Google.Cloud.Dlp.V2;
using Xunit;

namespace GoogleCloudSamples
{
    public class ReidentifyTableDataWithFpeTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public ReidentifyTableDataWithFpeTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestReidentifyTable()
        {
            var result = ReidentifyTableDataWithFpe.ReidentifyTableData(_fixture.ProjectId, _fixture.KeyName, _fixture.WrappedKey);
            var serializedObject = result.ToString();

            //After re-identification the employee ID will not be the same as input value (28777)
            Assert.DoesNotContain("28777", serializedObject);
            // Employee name field is not being re-identified so should match input value (Justin).
            Assert.Contains("Justin", serializedObject);
        }
    }
}
