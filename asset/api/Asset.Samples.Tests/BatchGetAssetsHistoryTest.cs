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
public class BatchGetAssetsHistoryTest
{
    private readonly AssetFixture _fixture;
    private readonly BatchGetAssetsHistorySample _sample;

    public BatchGetAssetsHistoryTest(AssetFixture fixture)
    {
        _fixture = fixture;
        _sample = new BatchGetAssetsHistorySample();
    }

    [Fact]
    public void TestBatchGetAssetsHistory()
    {
        // Run the sample code.
        string assetName = $"//storage.googleapis.com/{_fixture.BucketName}";

        _fixture.Retry.Eventually(() =>
        {
            // We substract 5 minutes from for the start time, because sometimes our clock and the server
            // clock are not synced.
            var result = _sample.BatchGetAssetsHistory(
                new string[] { assetName }, DateTimeOffset.UtcNow - TimeSpan.FromMinutes(5), _fixture.ProjectId);
            Assert.Contains(result.Assets, node => node.Asset.Name == assetName);
        });
    }
}
