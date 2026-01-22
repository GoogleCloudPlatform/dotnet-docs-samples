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

using System;
using System.IO;
using System.Collections.Generic;
using Xunit;
using Google.Cloud.SecretManager.V1;

[Collection(nameof(SecretManagerFixture))]
public class ListSecretVersionsWithFilterTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly ListSecretVersionsWithFilterSample _sample;

    public ListSecretVersionsWithFilterTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListSecretVersionsWithFilterSample();
    }

    [Fact]
    public void ListSecretVersionsWithFilter()
    {
        // Create the secret resource.
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());

        // Get the SecretName.
        SecretName secretName = secret.SecretName;

        // Run the code sample.
        SecretVersion secretVersion = _fixture.AddSecretVersion(secret);
        _fixture.DisableSecretVersion(secretVersion);

        // Capture console output
        StringWriter sw = new StringWriter();
        Console.SetOut(sw);

        // Run the sample code
        _sample.ListSecretVersionsWithFilter(
            projectId: secretName.ProjectId,
            secretId: secretName.SecretId);

        // Get the console output
        string consoleOutput = sw.ToString().Trim();

        // Assert that the output contains the expected message
        Assert.Contains($"Found secret version", consoleOutput);

        // Reset console
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);

        // Clean the created secret.
        _fixture.DeleteSecret(secretName);
    }
}
