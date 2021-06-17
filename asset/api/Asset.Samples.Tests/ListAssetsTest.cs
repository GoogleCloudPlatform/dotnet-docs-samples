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
public class ListAssetsTest
{
    private readonly AssetFixture _fixture;
    private readonly ListAssetsSample _sample;

    public ListAssetsTest(AssetFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListAssetsSample();
    }

    [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/1141")]
    public void TestListAssets()
    {
        // Run the sample code.
        var result = _sample.ListAssets(_fixture.ProjectId);

        string assetName = String.Format("//storage.googleapis.com/{0}", _fixture.BucketName);
        Assert.Contains(assetName, result.ToString());
    }
}
