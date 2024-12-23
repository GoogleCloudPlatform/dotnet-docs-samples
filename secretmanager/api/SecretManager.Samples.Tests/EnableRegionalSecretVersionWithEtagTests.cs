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
public class EnableRegionalSecretVersionWithEtagTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly EnableRegionalSecretVersionWithEtagSample _sample;

    public EnableRegionalSecretVersionWithEtagTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new EnableRegionalSecretVersionWithEtagSample();
    }

    [Fact]
    public void EnablesRegionalSecretVersionsWithEtag()
    {
        SecretName secretName = _fixture.SecretWithVersions.SecretName;
        SecretVersionName secretVersionName = _fixture.SecretVersionToEnable.SecretVersionName;

        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{secretName.LocationId}.rep.googleapis.com"
        }.Build();

        SecretVersion disabledSecretVersion = client.DisableSecretVersion(secretVersionName);
        string updatedEtag = disabledSecretVersion.Etag;

        SecretVersion secretVersion = _sample.EnableRegionalSecretVersionWithEtag(
          projectId: secretVersionName.ProjectId,
          locationId: secretVersionName.LocationId,
          secretId: secretVersionName.SecretId,
          secretVersionId: secretVersionName.SecretVersionId,
          etag: updatedEtag
        );
        Assert.Equal(SecretVersion.Types.State.Enabled, secretVersion.State);
    }
}
