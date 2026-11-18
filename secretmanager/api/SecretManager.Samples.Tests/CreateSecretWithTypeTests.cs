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
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class CreateSecretWithTypeTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly CreateSecretWithTypeSample _sample;

    public CreateSecretWithTypeTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateSecretWithTypeSample();
    }

    [Theory]
    [InlineData(Secret.Types.SecretType.AccessKey)]
    [InlineData(Secret.Types.SecretType.Certificate)]
    [InlineData(Secret.Types.SecretType.OtherDbCredentials)]
    [InlineData(Secret.Types.SecretType.Other)]
    public void CreatesSecretWithType(Secret.Types.SecretType secretType)
    {
        SecretName secretName = new SecretName(_fixture.ProjectId, _fixture.RandomId());

        try
        {
            // Run the sample code.
            Secret result = _sample.CreateSecretWithType(
              projectId: secretName.ProjectId, secretId: secretName.SecretId, secretType: secretType);

            // Assert that the secret was created with the requested type.
            Assert.Equal(secretName.SecretId, result.SecretName.SecretId);
            Assert.Equal(secretType, result.SecretType);
        }
        finally
        {
            // Clean the created secret.
            _fixture.DeleteSecret(secretName);
        }
    }
}
