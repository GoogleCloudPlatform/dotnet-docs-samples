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
public class SearchAllResourcesTest
{
    private readonly AssetFixture _fixture;
    private readonly SearchAllResourcesSample _sample;

    public SearchAllResourcesTest(AssetFixture fixture)
    {
        _fixture = fixture;
        _sample = new SearchAllResourcesSample();
    }

    [Fact]
    public void TestSearchAllResources()
    {
        // Run the sample code.
        string scope = String.Format("projects/{0}", _fixture.ProjectId);
        string query = String.Format("name:{0}", _fixture.DatasetId);
        var result = _sample.SearchAllResources(scope, query: query);

        Assert.Contains(_fixture.DatasetId, result.ToString());
    }
}
