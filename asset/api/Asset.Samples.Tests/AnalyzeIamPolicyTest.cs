/*
 * Copyright (c) 2020 Google LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using Xunit;

[Collection(nameof(AssetFixture))]
public class AnalyzeIamPolicyTest
{
    private readonly AssetFixture _fixture;
    private readonly AnalyzeIamPolicySample _sample;

    public AnalyzeIamPolicyTest(AssetFixture fixture)
    {
        _fixture = fixture;
        _sample = new AnalyzeIamPolicySample();
    }

    [Fact]
    public void TestAnalyzeIamPolicy()
    {
        // Run the sample code.
        string scope = $"projects/{_fixture.ProjectId}";
        string fullResourceName =
            $"//cloudresourcemanager.googleapis.com/projects/{_fixture.ProjectId}";
        var result = _sample.AnalyzeIamPolicy(scope, fullResourceName);

        Assert.Equal(fullResourceName, result.MainAnalysis.AnalysisQuery.ResourceSelector.FullResourceName);
    }
}
