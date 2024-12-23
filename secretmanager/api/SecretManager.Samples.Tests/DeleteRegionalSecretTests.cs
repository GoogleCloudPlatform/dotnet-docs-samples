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
public class DeleteRegionalSecretTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly DeleteRegionalSecretSample _sample;

    public DeleteRegionalSecretTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteRegionalSecretSample();
    }

    [Fact]
    public void DeletesRegionalSecrets()
    {
        SecretName secretName = _fixture.SecretToDelete.SecretName;
        _sample.DeleteRegionalSecret(projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId);

        // Create the Secret Manager Client with the regional endpoint.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{secretName.LocationId}.rep.googleapis.com"
        }.Build();

        Assert.Throws<Grpc.Core.RpcException>(() => client.GetSecret(secretName));
    }
}
