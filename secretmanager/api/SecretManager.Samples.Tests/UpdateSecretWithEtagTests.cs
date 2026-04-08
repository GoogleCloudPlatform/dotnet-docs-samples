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
using System;
using System.Collections.Generic;
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class UpdateSecretWithEtagTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly UpdateSecretWithEtagSample _sample;

    public UpdateSecretWithEtagTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new UpdateSecretWithEtagSample();
    }

    [Fact]
    public void UpdatesSecretWithEtag()
    {
        // Create a new secret for testing
        Secret initialSecret = _fixture.CreateSecret(_fixture.RandomId());

        // Verify the initial secret doesn't have our test label
        Assert.False(initialSecret.Labels.ContainsKey("secretmanager"));

        // Update the secret with ETag
        Secret updatedSecret = _sample.UpdateSecretWithEtag(
            projectId: initialSecret.SecretName.ProjectId,
            secretId: initialSecret.SecretName.SecretId);

        // Verify the secret was updated with our label
        Assert.True(updatedSecret.Labels.ContainsKey("secretmanager"));
        Assert.Equal("rocks", updatedSecret.Labels["secretmanager"]);

        // Clean up
        _fixture.DeleteSecret(initialSecret.SecretName);
    }
}
