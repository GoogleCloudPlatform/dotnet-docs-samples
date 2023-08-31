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

using Xunit;

namespace GoogleCloudSamples
{
    public class ReidentifyFreeTextFpeTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public ReidentifyFreeTextFpeTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestReidentifyWithFpe()
        {
            var finding = "9617256398";
            var text = $"My phone number is PHONE_TOKEN(10):{finding}";
            var expectedTransformFake = $"My phone number is {finding}";
            var result = ReidentifyFreeTextFpe.ReidentifyContent(_fixture.ProjectId, text, _fixture.KeyName, _fixture.WrappedKey);
            Assert.NotEqual(text, result.Item.Value);
            Assert.Equal(expectedTransformFake.Length, result.Item.Value.Length);
        }
    }
}