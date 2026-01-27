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
using Google.Protobuf.WellKnownTypes;
using System;
using System.IO;
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class UpdateSecretExpirationTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly UpdateSecretExpirationSample _sample;

    public UpdateSecretExpirationTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new UpdateSecretExpirationSample();
    }

    [Fact]
    public void UpdatesSecretExpiration()
    {
        // Capture console output
        StringWriter sw = new StringWriter();
        Console.SetOut(sw);

        // Create a new secret with no expiration time
        Secret secret = _fixture.CreateSecretWithExpiration();

        // Update the secret with an expiration time
        _sample.UpdateSecretExpiration(
            projectId: secret.SecretName.ProjectId,
            secretId: secret.SecretName.SecretId);

        // Get the console output
        string consoleOutput = sw.ToString().Trim();

        // Assert that the output contains the expected message
        Assert.Contains($"Updated secret", consoleOutput);

        // Reset console
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);

        // Clean up the created secret
        _fixture.DeleteSecret(secret.SecretName);
    }
}
