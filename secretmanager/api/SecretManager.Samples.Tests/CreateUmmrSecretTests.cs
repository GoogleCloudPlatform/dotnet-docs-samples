/*
 * Copyright 2022 Google LLC
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

using Xunit;
using Google.Cloud.SecretManager.V1;

[Collection(nameof(SecretManagerFixture))]
public class CreateUmmrSecretTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly CreateUmmrSecretSample _sample;

    public CreateUmmrSecretTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateUmmrSecretSample();
    }

    [Fact]
    public void CreatesUmmrSecrets()
    {
        SecretName secretName = _fixture.UmmrSecretToCreateName;
        string[] locations = {"us-east4"};
        Secret result = _sample.CreateUmmrSecret(
          projectId: secretName.ProjectId, secretId: secretName.SecretId, locations: locations);
        Assert.Equal(result.SecretName.SecretId, secretName.SecretId);
    }
}
