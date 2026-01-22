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
public class DeleteRegionalSecretAnnotationTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly DeleteRegionalSecretAnnotationSample _sample;

    public DeleteRegionalSecretAnnotationTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteRegionalSecretAnnotationSample();
    }

    [Fact]
    public void DeleteRegionalSecretAnnotation()
    {
        // Create a secret with annotations
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());
        SecretName secretName = secret.SecretName;

        // Get a key from the existing annotations
        string annotationKey = _fixture.AnnotationKey;

        // Verify the secret has annotations
        Assert.NotEmpty(secret.Annotations);
        Assert.True(secret.Annotations.ContainsKey(annotationKey));

        // Capture console output
        StringWriter sw = new StringWriter();
        Console.SetOut(sw);

        // Run the sample code to delete the annotation
        _sample.DeleteRegionalSecretAnnotation(
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
