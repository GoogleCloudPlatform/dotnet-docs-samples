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
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class GetSecretTypeTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly GetSecretTypeSample _sample;

    public GetSecretTypeTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new GetSecretTypeSample();
    }

    [Fact]
    public void GetsSecretType()
    {
        SecretName secretName = new SecretName(_fixture.ProjectId, _fixture.RandomId());
        _fixture.Client.CreateSecret(
          _fixture.ProjectName,
          secretName.SecretId,
          new Secret
          {
              Replication = new Replication { Automatic = new Replication.Types.Automatic() },
              SecretType = Secret.Types.SecretType.Certificate,
          });
        try
        {
            // Run the sample code.
            Secret result = _sample.GetSecretType(projectId: secretName.ProjectId, secretId: secretName.SecretId);

            Assert.Equal(secretName.SecretId, result.SecretName.SecretId);
            Assert.Equal(Secret.Types.SecretType.Certificate, result.SecretType);
        }
        finally
        {
            _fixture.DeleteSecret(secretName);
        }
    }
}
