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

using Google.Cloud.Dlp.V2;
using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class InspectTemplateDeleteTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public InspectTemplateDeleteTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestInspectTemplateDelete()
        {
            // Creation
            var name = $"my-inspect-template-{TestUtil.RandomName()}";
            var displayName = $"My display name {Guid.NewGuid()}";
            var description = $"My description {Guid.NewGuid()}";
            var fullName = $"projects/{Fixture.ProjectId}/inspectTemplates/{name}";

            var template = InspectTemplateCreate.Create(Fixture.ProjectId, name, displayName, description, Likelihood.Possible, 5, true);
            InspectTemplateDelete.Delete(Fixture.ProjectId, template.Name);

            var list = InspectTemplateList.List(Fixture.ProjectId);
            Assert.DoesNotContain(list, t => t.Name == fullName);
        }
    }
}
