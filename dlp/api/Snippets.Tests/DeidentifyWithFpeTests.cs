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
using Google.Cloud.Dlp.V2;
using Xunit;
using static Google.Cloud.Dlp.V2.CryptoReplaceFfxFpeConfig.Types;

namespace GoogleCloudSamples
{
    public class DeidentifyWithFpeTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public DeidentifyWithFpeTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestDeidentifyWithFpe()
        {
            var finding = "372819127";
            var testData = $"My SSN is {finding}";
            var findingLength = finding.Length;
            var expectedTransformFake = $"My SSN is TOKEN({finding.Length}):{finding}";
            var infoTypes = new InfoType[] { new InfoType { Name = "US_SOCIAL_SECURITY_NUMBER" } };
            var result = DeidentifyWithFpe.Deidentify(Fixture.ProjectId, testData, infoTypes, Fixture.KeyName, Fixture.WrappedKey, FfxCommonNativeAlphabet.Numeric);
            Console.WriteLine($"Deid result: {result}");
            Assert.NotEqual(testData, result.Item.Value);
            Assert.Equal(expectedTransformFake.Length, result.Item.Value.Length);
        }
    }
}
