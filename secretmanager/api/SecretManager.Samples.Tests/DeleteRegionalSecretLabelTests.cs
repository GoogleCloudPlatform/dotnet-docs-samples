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

[Collection(nameof(RegionalSecretManagerFixture))]
public class DeleteRegionalSecretLabelTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly DeleteRegionalSecretLabelSample _sample;

    public DeleteRegionalSecretLabelTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteRegionalSecretLabelSample();
    }

    [Fact]
    public void DeleteRegionalSecretLabel()
    {
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());
        SecretName secretName = secret.SecretName;

        // Verify the secret has labels
        Assert.NotEmpty(secret.Labels);

        // Capture console output
        StringWriter sw = new StringWriter();
        Console.SetOut(sw);

        // Run the sample code to delete all labels
        _sample.DeleteRegionalSecretLabel(

            projectId: secretName.ProjectId,
            locationId: secretName.LocationId,
            secretId: secretName.SecretId);

        // Get the console output
        string consoleOutput = sw.ToString().Trim();

        // Assert that the output contains the expected messages
        Assert.Contains($"Updated secret:", consoleOutput);

        // Reset console
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);

        // Clean the created secret.
        _fixture.DeleteSecret(secretName);
    }
}
