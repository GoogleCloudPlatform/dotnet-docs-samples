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
using Google.Protobuf.WellKnownTypes;
using System;
using System.IO;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class DeleteRegionalSecretExpirationTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly DeleteRegionalSecretExpirationSample _deleteSample;

    public DeleteRegionalSecretExpirationTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _deleteSample = new DeleteRegionalSecretExpirationSample();
    }

    [Fact]
    public void DeletesRegionalSecretExpiration()
    {
        Secret initialSecret = _fixture.CreateSecretWithExpireTime();

        // Delete the expiration time
        Secret result = _deleteSample.DeleteRegionalSecretExpiration(
            projectId: initialSecret.SecretName.ProjectId,
            secretId: initialSecret.SecretName.SecretId,
            locationId: initialSecret.SecretName.LocationId);

        Assert.Null(result.ExpireTime);
        // Clean the created secret
        _fixture.DeleteSecret(initialSecret.SecretName);
    }
}
