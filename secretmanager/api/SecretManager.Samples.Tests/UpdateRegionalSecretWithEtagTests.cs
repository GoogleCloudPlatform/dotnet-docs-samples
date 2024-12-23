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
        // Create the secret and add secret version.
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());
        SecretName secretName = secret.SecretName;
        SecretVersion secretVersion = _fixture.AddSecretVersion(secret);

<<<<<<< HEAD
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{secretName.LocationId}.rep.googleapis.com"
        }.Build();

        Secret secret = client.GetSecret(secretName);
        string etag = secret.Etag;

        Secret result = _sample.UpdateRegionalSecretWithEtag(
          projectId: secretName.ProjectId,
          locationId: secretName.LocationId,
          secretId: secretName.SecretId,
          etag: etag
        );
        Assert.Equal("rocks", result.Labels["secretmanager"]);
=======
        // Get the etag associated with secret version.
        string etag = _fixture.client.GetSecret(secretName).Etag;

        // Run the code sample.
        Secret result = _sample.UpdateRegionalSecretWithEtag(
          projectId: secretName.ProjectId,
          locationId: secretName.LocationId,
          secretId: secretName.SecretId,
          etag: etag
        );

        // Assert that the secret label was correctly updated.
        Assert.Equal("stones", result.Labels["secretmanager"]);

        // Clean the created resources
        _fixture.DeleteSecret(secretName);
>>>>>>> 95d07aff (chore: Add SecretManager service regional code samples)
    }
}
