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

[Collection(nameof(SecretManagerFixture))]
public class DeleteSecretExpirationTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly DeleteSecretExpirationSample _deleteSample;

    public DeleteSecretExpirationTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _deleteSample = new DeleteSecretExpirationSample();
    }

    [Fact]
    public void DeletesSecretExpiration()
    {
        // First, create a secret with an expiration time
        Secret secret = _fixture.CreateSecretWithExpiration();
        try
        {
            // Delete the expiration time
            Secret result = _deleteSample.DeleteSecretExpiration(
                projectId: secret.SecretName.ProjectId, secretId: secret.SecretName.SecretId);

            Assert.Null(result.ExpireTime);
        }
        finally
        {
            // Clean up the created secret
            _fixture.DeleteSecret(secret.SecretName);
        }
    }
}
