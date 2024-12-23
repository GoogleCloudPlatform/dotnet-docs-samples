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
public class DestroyRegionalSecretVersionWithEtagTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly DestroyRegionalSecretVersionWithEtagSample _sample;

    public DestroyRegionalSecretVersionWithEtagTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DestroyRegionalSecretVersionWithEtagSample();
    }

    [Fact]
    public void DestroysRegionalSecretVersionsWithEtag()
    {
        SecretName secretName = _fixture.SecretWithVersions.SecretName;
        SecretVersionName secretVersionName = _fixture.SecretVersionToDestroyWithEtag.SecretVersionName;

        string etag = _fixture.SecretVersionToDestroyWithEtag.Etag;

        SecretVersion secretVersion = _sample.DestroyRegionalSecretVersionWithEtag(
          projectId: secretVersionName.ProjectId, locationId: secretVersionName.LocationId, secretId: secretVersionName.SecretId, secretVersionId: secretVersionName.SecretVersionId, etag: etag);
        Assert.Equal(SecretVersion.Types.State.Destroyed, secretVersion.State);
    }
}
