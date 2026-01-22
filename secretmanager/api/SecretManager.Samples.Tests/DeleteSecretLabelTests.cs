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
using System.IO;
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class DeleteSecretLabelTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly DeleteSecretLabelSample _sample;

    public DeleteSecretLabelTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteSecretLabelSample();
    }

    [Fact]
    public void DeleteSecretLabel()
    {
        // Create the secret.
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());

        // Get the secretName from the created secret.
        SecretName secretName = secret.SecretName;

        // Capture console output
        StringWriter sw = new StringWriter();
        Console.SetOut(sw);

        // Call the code sample function to delete the label
        _sample.DeleteSecretLabel(
          projectId: secretName.ProjectId, secretId: secretName.SecretId);

        // Get the console output
        string consoleOutput = sw.ToString().Trim();

        // Assert that the output contains the expected message
        Assert.Contains("Updated secret:", consoleOutput);

        // Reset console
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }
}
