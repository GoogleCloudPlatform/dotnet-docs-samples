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
public class CreateSecretWithExpirationTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly CreateSecretWithExpirationSample _sample;

    public CreateSecretWithExpirationTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateSecretWithExpirationSample();
    }

    [Fact]
    public void CreatesSecretWithExpiration()
    {
        // Get the SecretName to create Secret.
        SecretName secretName = new SecretName(_fixture.ProjectId, _fixture.RandomId());
        try
        {
            // Create the secret with expiration.
            Secret result = _sample.CreateSecretWithExpiration(
              projectId: secretName.ProjectId, secretId: secretName.SecretId);

            Assert.NotNull(result.ExpireTime);
        }
        finally
        {
            // Clean the created secret.
            _fixture.DeleteSecret(secretName);
        }
    }
}
