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
public class CreateUserManagedReplicationSecretTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly CreateUserManagedReplicationSecretSample _sample;

    public CreateUserManagedReplicationSecretTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateUserManagedReplicationSecretSample();
    }

    [Fact]
    public void CreatesUmmrSecrets()
    {
        SecretName secretName = _fixture.UserManagedReplicationSecretName;
        string[] locations = {"us-east1", "us-east4", "us-west1"};
        Secret result = _sample.CreateUserManagedReplicationSecret(
          projectId: secretName.ProjectId, secretId: secretName.SecretId, locations: locations);
        Assert.Equal(result.SecretName.SecretId, secretName.SecretId);
        Assert.Contains(result.Replication.UserManaged.Replicas, r => r.Location == "us-east1");
        Assert.Contains(result.Replication.UserManaged.Replicas, r => r.Location == "us-east4");
        Assert.Contains(result.Replication.UserManaged.Replicas, r => r.Location == "us-west1");
    }
}
