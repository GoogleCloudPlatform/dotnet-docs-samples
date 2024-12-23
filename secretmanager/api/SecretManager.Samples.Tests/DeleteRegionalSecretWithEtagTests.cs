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
public class DeleteRegionalSecretWithEtagTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly DeleteRegionalSecretWithEtagSample _sample;

    public DeleteRegionalSecretWithEtagTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteRegionalSecretWithEtagSample();
    }

    [Fact]
    public void DeletesRegionalSecretsWithEtag()
    {
        // Create the secret.
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());

<<<<<<< HEAD
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{secretName.LocationId}.rep.googleapis.com"
        }.Build();
=======
        // Get the secret name.
        SecretName secretName = secret.SecretName;
>>>>>>> 95d07aff (chore: Add SecretManager service regional code samples)

        // Get the secret from the cloud.
        Secret secretFetched = _fixture.client.GetSecret(secretName);

<<<<<<< HEAD
        _sample.DeleteRegionalSecretWithEtag(
          projectId: secretName.ProjectId,
          locationId: secretName.LocationId,
          secretId: secretName.SecretId,
          etag: updatedEtag
        );
=======
        // Set the updated etag fetched.
        string updatedEtag = secretFetched.Etag;
>>>>>>> 95d07aff (chore: Add SecretManager service regional code samples)

        // Run the sample to delete the secret with etag.
        _sample.DeleteRegionalSecretWithEtag(
          projectId: secretName.ProjectId,
          locationId: secretName.LocationId,
          secretId: secretName.SecretId,
          etag: updatedEtag
        );

        // Assert that the secret was deleted and not found.
        Assert.Throws<Grpc.Core.RpcException>(() => _fixture.client.GetSecret(secretName));
    }
}
