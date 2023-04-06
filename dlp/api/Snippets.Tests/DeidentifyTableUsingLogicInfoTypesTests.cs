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

using Google.Cloud.Dlp.V2;
using Xunit;

namespace GoogleCloudSamples
{
    public class DeidentifyTableUsingLogicInfoTypesTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public DeidentifyTableUsingLogicInfoTypesTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestDeidentifyTableUsingLogic()
        {
            var patientName1 = "Charles Dickens"; // Person name with age greater than 89
            var patientName2 = "Jane Austin"; // Person name with age less than 89
            var infoTypes = new InfoType[] { new InfoType { Name = "PERSON_NAME" } };
            var result = DeidentifyTableUsingLogicInfoTypes.Deidentify(_fixture.ProjectId, infoTypes: infoTypes);
            var serializedObject = result.ToString();
            Assert.DoesNotContain(patientName1, serializedObject);
            Assert.Contains("[PERSON_NAME]", serializedObject);
            Assert.Contains(patientName2, serializedObject);
        }
    }
}