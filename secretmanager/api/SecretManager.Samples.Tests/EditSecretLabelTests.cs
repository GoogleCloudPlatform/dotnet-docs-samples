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
public class EditSecretLabelTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly EditSecretLabelSample _sample;

    public EditSecretLabelTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new EditSecretLabelSample();
    }

    [Fact]
    public void EditSecretLabel()
    {
        // Get the SecretName to create Secret.
        SecretName secretName = _fixture.Secret.SecretName;


        // Capture console output
        StringWriter sw = new StringWriter();
        Console.SetOut(sw);

        // Call the code sample function.
        _sample.EditSecretLabel(
          projectId: secretName.ProjectId, secretId: secretName.SecretId);

        // Get the console output
        string consoleOutput = sw.ToString().Trim();

        Assert.Contains($"Updated secret: ", consoleOutput);

        // Reset console
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }
}
