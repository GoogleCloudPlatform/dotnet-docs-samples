/*
 * Copyright (c) 2018 Google LLC.
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
public class ExportAssetsTest
{
    private readonly AssetFixture _fixture;
    private readonly ExportAssetsSample _sample;

    public ExportAssetsTest(AssetFixture fixture)
    {
        _fixture = fixture;
        _sample = new ExportAssetsSample();
    }

    [Fact]
    public void TestExportAssets()
    {
        // Run the sample code.
        var result = _sample.ExportAssets(_fixture.BucketName, _fixture.ProjectId);

        string expectedOutput = String.Format("\"outputConfig\": {{ \"gcsDestination\": {{ \"uri\": \"gs://{0}/my-assets.txt\" }} }}", _fixture.BucketName);
        Assert.Contains(expectedOutput, result.ToString());
    }
}
