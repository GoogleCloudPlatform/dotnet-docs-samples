/*
 * Copyright 2024 Google LLC
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
<<<<<<< HEAD
using System;
using System.IO;
=======
using System.Collections.Generic;
>>>>>>> 95d07aff (chore: Add SecretManager service regional code samples)
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class ListRegionalSecretVersionsTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly ListRegionalSecretVersionsSample _sample;

    public ListRegionalSecretVersionsTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListRegionalSecretVersionsSample();
    }

    [Fact]
    public void ListsRegionalSecretVersions()
    {
<<<<<<< HEAD
        // Redirect console output to a StringWriter
        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

=======
        // Get the secret name.
>>>>>>> 95d07aff (chore: Add SecretManager service regional code samples)
        SecretName secretName = _fixture.Secret.SecretName;

        // Run the code sample.
        List<SecretVersion> result = _sample.ListRegionalSecretVersions(
            projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId);

<<<<<<< HEAD
        // Get the console output and restore the original console output stream
        string output = stringWriter.ToString();
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));

        // Construct the expected regex pattern.
        string pattern = $@"Got regional secret version : projects\/{secretName.ProjectId}\/locations\/{secretName.LocationId}\/secrets\/[\w-]+";

        // Assert that the output matches the pattern.
        Assert.Matches(pattern, output);
=======
        // Assert that the secret list is not empty.
        Assert.NotEmpty(result);
>>>>>>> 95d07aff (chore: Add SecretManager service regional code samples)
    }
}
