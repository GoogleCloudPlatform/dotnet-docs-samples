/*
 * Copyright 2026 Google LLC
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

using Google.Cloud.SecretManager.V1;
using System;
using System.IO;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class CreateRegionalSecretWithExpireTimeTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly CreateRegionalSecretWithExpireTimeSample _sample;

    public CreateRegionalSecretWithExpireTimeTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateRegionalSecretWithExpireTimeSample();
    }

    [Fact]
    public void CreatesRegionalSecretWithExpireTime()
    {
        // Get the SecretName from the set ProjectId & LocationId.
        SecretName secretName = SecretName.FromProjectLocationSecret(
            _fixture.ProjectId, _fixture.LocationId, _fixture.RandomId());

        // Run the code sample.
        Secret result = _sample.CreateRegionalSecretWithExpireTime(
            projectId: secretName.ProjectId,
            secretId: secretName.SecretId,
            locationId: secretName.LocationId);

        Assert.NotEmpty(result.ExpireTime.ToString());
        // Clean the created secret.
        _fixture.DeleteSecret(secretName);
    }
}
