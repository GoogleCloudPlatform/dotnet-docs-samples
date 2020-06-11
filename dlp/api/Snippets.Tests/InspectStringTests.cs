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

using System;
using System.Linq;
using Google.Cloud.Dlp.V2;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectStringTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public InspectStringTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestInspectString_PersonName()
        {
            string testData = "The name Robert is very common.";
            var infoTypes = new InfoType[] { new InfoType { Name = "PERSON_NAME" } };
            var customInfoTypes = new CustomInfoType[] { };
            var response = InspectString.Inspect(Fixture.ProjectId, testData, Likelihood.Possible.ToString(), 5, true, infoTypes, customInfoTypes);

            Assert.Contains(response.Result.Findings, f => f.Quote == "Robert");
        }

        [Fact]
        public void TestInspectString_NoResults()
        {
            string testData = "She sells sea shells by the sea shore.";
            var infoTypes = new InfoType[] { new InfoType { Name = "PERSON_NAME" } };
            var customInfoTypes = new CustomInfoType[] { };
            var response = InspectString.Inspect(Fixture.ProjectId, testData, Likelihood.Possible.ToString(), 5, true, infoTypes, customInfoTypes);

            Assert.True(!response.Result.Findings.Any());
        }

        [Fact]
        public void TestInspectString_CustomRegex()
        {
            string testData = "My name is Robert and my phone number is (425) 634-9233.";
            var infoTypes = new InfoType[] { };
            var customRegex = new CustomInfoType[]
            {
                new CustomInfoType
                {
                    InfoType = new InfoType
                    {
                        Name = $"CUSTOM_REGEX_{Guid.NewGuid()}"
                    },
                    Regex = new CustomInfoType.Types.Regex
                    {
                        Pattern = "\\(\\d{3}\\) \\d{3}-\\d{4}"
                    }
                }
            };
            var response = InspectString.Inspect(Fixture.ProjectId, testData, Likelihood.Possible.ToString(), 5, true, infoTypes, customRegex);

            Assert.Contains(response.Result.Findings, f => f.Quote == "(425) 634-9233");
        }

        [Fact]
        public void TestInspectString_CustomDictionary()
        {
            string testData = "My name is Robert and my phone number is (425) 634-9233.";
            var infoTypes = new InfoType[] { };
            var customDictionary = new CustomInfoType[]
            {
                new CustomInfoType
                {
                    InfoType = new InfoType
                    {
                        Name = $"CUSTOM_DICTIONARY_{Guid.NewGuid()}"
                    },
                    Dictionary = new CustomInfoType.Types.Dictionary
                    {
                        WordList = new CustomInfoType.Types.Dictionary.Types.WordList
                        {
                            Words = { new string[] { "Robert" } }
                        }
                    }
                }
            };
            var response = InspectString.Inspect(Fixture.ProjectId, testData, Likelihood.Possible.ToString(), 5, true, infoTypes, customDictionary);

            Assert.Contains(response.Result.Findings, f => f.Quote == "Robert");
        }

        [Fact]
        public void TestInspectString_CustomInfoType_NoResults()
        {
            string testData = "She sells sea shells by the sea shore.";
            var infoTypes = new InfoType[] { };
            var customInfoType = new CustomInfoType[]
            {
                new CustomInfoType
                {
                    InfoType = new InfoType
                    {
                        Name = $"CUSTOM_REGEX_{Guid.NewGuid()}"
                    },
                    Regex = new CustomInfoType.Types.Regex
                    {
                        Pattern = "\\(\\d{3}\\) \\d{3}-\\d{4}"
                    },
                    Dictionary = new CustomInfoType.Types.Dictionary
                    {
                        WordList = new CustomInfoType.Types.Dictionary.Types.WordList
                        {
                            Words = { new string[] { "Robert" } }
                        }
                    }
                }
            };
            var response = InspectString.Inspect(Fixture.ProjectId, testData, Likelihood.Possible.ToString(), 5, true, infoTypes, customInfoType);

            Assert.True(!response.Result.Findings.Any());
        }
    }
}
