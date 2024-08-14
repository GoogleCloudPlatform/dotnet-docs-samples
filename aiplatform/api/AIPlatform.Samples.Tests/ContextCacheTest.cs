/*
 * Copyright 2024 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Cloud.AIPlatform.V1Beta1;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(AIPlatformFixture))]
public class ContextCacheTest
{
    private readonly AIPlatformFixture _fixture;
    private readonly CreateContextCache _createSample;
    private readonly UseContextCache _useSample;

    private readonly GetContextCache _getSample;
    private readonly UpdateContextCache _updateSample;
    private readonly DeleteContextCache _deleteSample;


    public ContextCacheTest(AIPlatformFixture fixture)
    {
        _fixture = fixture;
        _createSample = new CreateContextCache();
        _useSample = new UseContextCache();
        _getSample = new GetContextCache();
        _updateSample = new UpdateContextCache();
        _deleteSample = new DeleteContextCache();
    }

    [Fact]
    public async Task TestContextCache()
    {
        // Create context cache
        CachedContentName name = await _createSample.Create(_fixture.ProjectId);
        Assert.NotNull(name);

        // Use context cache
        string response = await _useSample.Use(_fixture.ProjectId, name);
        Assert.NotNull(response);

        // Get context cache
        var cache = await _getSample.Get(name);
        Assert.Equal(name, cache.CachedContentName);

        // Update context cache
        Timestamp expireTime = await _updateSample.UpdateExpireTime(name);
        Assert.NotEqual(cache.ExpireTime, expireTime);

        // Delete context cache
        await _deleteSample.Delete(name);
    }
}
