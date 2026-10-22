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
using Grpc.Core;
using System;
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class DestroySecretVersionWithEtagTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly DestroySecretVersionWithEtagSample _sample;

    public DestroySecretVersionWithEtagTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DestroySecretVersionWithEtagSample();
    }

    [Fact]
    public void DestroysSecretVersionWithEtag()
    {
        // Create a new secret with a random ID
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());
        // Add a version to the secret
        SecretVersion version = _fixture.AddSecretVersion(secret);

        // Verify the initial version is enabled
        Assert.Equal(SecretVersion.Types.State.Enabled, version.State);

        // Destroy the secret version with ETag
        SecretVersion destroyedVersion = _sample.DestroySecretVersionWithEtag(
            projectId: secret.SecretName.ProjectId,
            secretId: secret.SecretName.SecretId,
            versionId: version.SecretVersionName.SecretVersionId);

        // Verify the version was destroyed
        Assert.Equal(SecretVersion.Types.State.Destroyed, destroyedVersion.State);

        // Verify we can't access the secret payload anymore
        SecretManagerServiceClient client = _fixture.Client;
        var exception = Assert.Throws<RpcException>(() =>
            client.AccessSecretVersion(version.SecretVersionName));

        Assert.Equal(StatusCode.FailedPrecondition, exception.StatusCode);

        // Clean up
        _fixture.DeleteSecret(secret.SecretName);
    }
}
