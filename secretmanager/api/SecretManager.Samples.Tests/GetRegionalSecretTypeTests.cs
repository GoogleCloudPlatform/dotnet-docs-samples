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

[Collection(nameof(RegionalSecretManagerFixture))]
public class GetRegionalSecretTypeTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly GetRegionalSecretTypeSample _sample;

    public GetRegionalSecretTypeTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new GetRegionalSecretTypeSample();
    }

    [Fact]
    public void GetsRegionalSecretType()
    {
        SecretName secretName = SecretName.FromProjectLocationSecret(_fixture.ProjectId, _fixture.LocationId, _fixture.RandomId());
        _fixture.Client.CreateSecret(
          new Google.Api.Gax.ResourceNames.LocationName(_fixture.ProjectId, _fixture.LocationId),
          secretName.SecretId,
          new Secret { SecretType = Secret.Types.SecretType.CloudSqlDbCredentials });
        try
        {
            // Run the sample code.
            Secret result = _sample.GetRegionalSecretType(
              projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId);

            Assert.Equal(secretName.SecretId, result.SecretName.SecretId);
            Assert.Equal(Secret.Types.SecretType.CloudSqlDbCredentials, result.SecretType);
        }
        finally
        {
            _fixture.DeleteSecret(secretName);
        }
    }
}
