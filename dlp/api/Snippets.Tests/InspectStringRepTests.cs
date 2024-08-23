// Copyright 2024 Google Inc.
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
using System.Linq;
using Google.Cloud.Dlp.V2;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectStringRepTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public InspectStringRepTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestInspectString_PersonName()
        {
            string testData = "The name Robert is very common.";
            string repLocation = "us-west1";
            var infoTypes = new InfoType[] { new InfoType { Name = "PERSON_NAME" } };
            var customInfoTypes = new CustomInfoType[] { };
            var response = InspectStringRep.Inspect(Fixture.ProjectId, repLocation, testData, Likelihood.Possible.ToString(), 5, true, infoTypes, customInfoTypes);

            Assert.Contains(response.Result.Findings, f => f.Quote == "Robert");
        }
    }
}
