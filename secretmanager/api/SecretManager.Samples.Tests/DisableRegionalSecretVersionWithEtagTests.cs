/*
 * Copyright 2020 Google LLC
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
public class DisableRegionalSecretVersionWithEtagTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly DisableRegionalSecretVersionWithEtagSample _sample;

    public DisableRegionalSecretVersionWithEtagTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DisableRegionalSecretVersionWithEtagSample();
    }

    [Fact]
    public void DisablesRegionalSecretVersionsWithEtag()
    {
        SecretName secretName = _fixture.SecretWithVersions.SecretName;
        SecretVersionName secretVersionName = _fixture.SecretVersionToDisable.SecretVersionName;

        // Create the Secret Manager Client with the regional endpoint.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{secretName.LocationId}.rep.googleapis.com"
        }.Build();

        SecretVersion enabledSecretVersion = client.EnableSecretVersion(secretVersionName);
        string updatedEtag = enabledSecretVersion.Etag;

        SecretVersion secretVersion = _sample.DisableRegionalSecretVersionWithEtag(
          projectId: secretVersionName.ProjectId, locationId: secretVersionName.LocationId, secretId: secretVersionName.SecretId, secretVersionId: secretVersionName.SecretVersionId, etag: updatedEtag);
        Assert.Equal(SecretVersion.Types.State.Disabled, secretVersion.State);
    }
}
