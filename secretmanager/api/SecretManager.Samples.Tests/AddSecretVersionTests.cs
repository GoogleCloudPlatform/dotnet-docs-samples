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
        string data = "my secret data";
        SecretName secretName = _fixture.Secret.SecretName;
        SecretVersion secretVersion = _sample.AddSecretVersion(
          projectId: secretName.ProjectId, secretId: secretName.SecretId,
          data: data);

        SecretManagerServiceClient client = SecretManagerServiceClient.Create();
        AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersion.SecretVersionName);
        Assert.Equal(data, result.Payload.Data.ToStringUtf8());
    }
}
