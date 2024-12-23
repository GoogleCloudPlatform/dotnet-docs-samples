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
public class UpdateRegionalSecretWithEtagTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly UpdateRegionalSecretWithEtagSample _sample;

    public UpdateRegionalSecretWithEtagTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new UpdateRegionalSecretWithEtagSample();
    }

    [Fact]
    public void UpdatesRegionalSecretsWithEtag()
    {
        SecretName secretName = _fixture.Secret.SecretName;

        // Create the Secret Manager Client with the regional endpoint.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{secretName.LocationId}.rep.googleapis.com"
        }.Build();

        Secret secret = client.GetSecret(secretName);
        string updatedEtag = secret.Etag;

        Secret result = _sample.UpdateRegionalSecretWithEtag(projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId, etag: updatedEtag);
        Assert.Equal("rocks", result.Labels["secretmanager"]);
    }
}
