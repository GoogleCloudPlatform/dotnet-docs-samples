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

[Collection(nameof(SecretManagerFixture))]
public class DeleteSecretTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly DeleteSecretSample _sample;

    public DeleteSecretTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteSecretSample();
    }

    [Fact]
    public void DeletesSecrets()
    {
        // Create the secret.
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());

        // Get the secretName from the created secret.
        SecretName secretName = secret.SecretName;

        // Call the code sample function.
        _sample.DeleteSecret(projectId: secretName.ProjectId, secretId: secretName.SecretId);

        // Assert expected behavior is seen.
        SecretManagerServiceClient client = _fixture.Client;
        Assert.Throws<Grpc.Core.RpcException>(() => client.GetSecret(secretName));
    }
}
