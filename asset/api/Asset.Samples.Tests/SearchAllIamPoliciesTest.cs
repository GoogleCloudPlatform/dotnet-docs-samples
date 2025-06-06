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
public class SearchAllIamPoliciesTest
{
    private readonly AssetFixture _fixture;
    private readonly SearchAllIamPoliciesSample _sample;

    public SearchAllIamPoliciesTest(AssetFixture fixture)
    {
        _fixture = fixture;
        _sample = new SearchAllIamPoliciesSample();
    }

    [Fact]
    public void TestSearchAllIamPolicies()
    {
        // Run the sample code.
        string scope = $"projects/{_fixture.ProjectId}";
        // On the test we use an empty query to make sure we find some policy.
        var result = _sample.SearchAllIamPolicies(scope, query: "");

        Assert.NotEmpty(result.Results);
    }
}
