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
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class EnableSecretVersionWithEtagTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly EnableSecretVersionWithEtagSample _sample;

    public EnableSecretVersionWithEtagTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new EnableSecretVersionWithEtagSample();
    }

    [Fact]
    public void EnablesSecretVersionWithEtag()
    {
        // Create a new secret with a random ID
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());
        SecretVersion version = _fixture.AddSecretVersion(secret);

        // First disable the version so we can test enabling it
        _fixture.DisableSecretVersion(version);

        // Enable the secret version with ETag
        SecretVersion enabledVersion = _sample.EnableSecretVersionWithEtag(
            projectId: secret.SecretName.ProjectId,
            secretId: secret.SecretName.SecretId,
            versionId: version.SecretVersionName.SecretVersionId);

        // Verify the version was enabled
        Assert.Equal(SecretVersion.Types.State.Enabled, enabledVersion.State);

        // Clean up
        _fixture.DeleteSecret(secret.SecretName);
    }
}
