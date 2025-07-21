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
public class CreateSecretTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly CreateSecretSample _sample;

    public CreateSecretTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateSecretSample();
    }

    [Fact]
    public void CreatesSecrets()
    {
        // Get the SecretName.
        SecretName secretName = new SecretName(_fixture.ProjectId, _fixture.RandomId());

        // Run the code sample.
        Secret result = _sample.CreateSecret(
          projectId: secretName.ProjectId, secretId: secretName.SecretId);

        // Assert expected result is observed.
        Assert.Equal(result.SecretName.SecretId, secretName.SecretId);

        // Cleanup the created resource.
        _fixture.DeleteSecret(secretName);
    }
}
