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

using Xunit;
using Google.Cloud.SecretManager.V1;

[Collection(nameof(SecretManagerFixture))]
public class AddSecretVersionTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly AddSecretVersionSample _sample;

    public AddSecretVersionTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new AddSecretVersionSample();
    }

    [Fact]
    public void AddsSecretVersions()
    {
        // Get the payload data.
        string data = "my secret data";

        // Create the secret resource.
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());

        // Get the SecretName.
        SecretName secretName = secret.SecretName;

        // Run the code sample.
        SecretVersion secretVersion = _sample.AddSecretVersion(
          projectId: secretName.ProjectId, secretId: secretName.SecretId,
          data: data);

        // Assert expected result is observed.
        SecretManagerServiceClient client = _fixture.Client;
        AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersion.SecretVersionName);
        Assert.Equal(data, result.Payload.Data.ToStringUtf8());

        // Cleanup the created resource.
        _fixture.DeleteSecret(secretName);
    }
}
