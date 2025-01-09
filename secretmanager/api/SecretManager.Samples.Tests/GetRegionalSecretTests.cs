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

using Google.Cloud.SecretManager.V1;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class GetRegionalSecretTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly GetRegionalSecretSample _sample;

    public GetRegionalSecretTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new GetRegionalSecretSample();
    }

    [Fact]
    public void GetsRegionalSecrets()
    {
        SecretName secretName = _fixture.Secret.SecretName;
        Secret result = _sample.GetRegionalSecret(
          projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId);
        Assert.Equal(result.SecretName.SecretId, secretName.SecretId);
    }
}
