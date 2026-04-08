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
using Grpc.Core;
using System;
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class DeleteSecretWithEtagTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly DeleteSecretWithEtagSample _sample;

    public DeleteSecretWithEtagTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteSecretWithEtagSample();
    }

    [Fact]
    public void DeletesSecretWithEtag()
    {
        // Create a new secret for testing with a random ID
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());
        string projectId = secret.SecretName.ProjectId;
        string secretId = secret.SecretName.SecretId;

        // Delete the secret using the sample
        _sample.DeleteSecretWithEtag(projectId, secretId);

        // Assert expected behavior is seen.
        SecretManagerServiceClient client = _fixture.Client;
        Assert.Throws<Grpc.Core.RpcException>(() => client.GetSecret(secret.SecretName));

    }
}
