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

using Xunit;

namespace GoogleCloudSamples
{
    public class DeidentifyWithExceptionListTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public DeidentifyWithExceptionListTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestDeidentifyWithExceptionList()
        {
            var testString = "jack@example.org accessed customer record of user5@example.com";
            var expectedString = "jack@example.org accessed customer record of [EMAIL_ADDRESS]";
            var result = DeidentifyWithExceptionList.Deidentify(Fixture.ProjectId, testString);

            Assert.Equal(expectedString, result.Item.Value);
        }
    }
}
