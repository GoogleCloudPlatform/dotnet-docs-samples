/*
 * Copyright 2024 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
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
public class ListRegionalSecretsTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly ListRegionalSecretsSample _sample;

    public ListRegionalSecretsTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListRegionalSecretsSample();
    }

    [Fact]
    public void ListsRegionalSecrets()
    {
        // Redirect console output to a StringWriter
        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        SecretName secretName = _fixture.Secret.SecretName;
        _sample.ListRegionalSecrets(projectId: secretName.ProjectId, locationId: secretName.LocationId);

        // Get the console output and restore the original console output stream
        string output = stringWriter.ToString();
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));

        // Construct the expected regex pattern.
        string pattern = $@"Got regional secret : projects\/{secretName.ProjectId}\/locations\/{secretName.LocationId}\/secrets\/[\w-]+";

        // Assert that the output matches the pattern.
        Assert.Matches(pattern, output);
    }
}
