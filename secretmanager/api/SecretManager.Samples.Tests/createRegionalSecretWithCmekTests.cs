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
public class CreateRegionalSecretWithCmekTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly CreateRegionalSecretWithCmekSample _sample;

    public CreateRegionalSecretWithCmekTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateRegionalSecretWithCmekSample();
    }

    [Fact]
    public void CreatesRegionalSecretWithCmek()
    {
        // Skip the test if no regional KMS key is available
        if (string.IsNullOrEmpty(_fixture.KmsKeyName))
        {
            return;
        }

        // Get the SecretName from the set ProjectId & LocationId.
        SecretName secretName = SecretName.FromProjectLocationSecret(_fixture.ProjectId, _fixture.LocationId, _fixture.RandomId());

        // Create the regional secret with CMEK.
        Secret result = _sample.CreateRegionalSecretWithCmek(
            projectId: secretName.ProjectId,
            locationId: secretName.LocationId,
            secretId: secretName.SecretId,
            kmsKeyName: _fixture.KmsKeyName);

        Assert.Equal(result.CustomerManagedEncryption.KmsKeyName, _fixture.KmsKeyName);

        // Clean the created secret.
        _fixture.DeleteSecret(secretName);

    }
}
