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
using System.Collections.Generic;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class ListRegionalSecretVersionsWithFilterTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly ListRegionalSecretVersionsWithFilterSample _sample;

    public ListRegionalSecretVersionsWithFilterTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListRegionalSecretVersionsWithFilterSample();
    }

    [Fact]
    public void ListsRegionalSecretVersions()
    {
        string filter = "name:csharp";

        // Get the secret name.
        SecretName secretName = _fixture.Secret.SecretName;

        // Run the code sample.
        List<SecretVersion> result = _sample.ListRegionalSecretVersionsWithFilter(
            projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId, filter: filter);

        // Assert that the result is not empty.
        Assert.NotEmpty(result);
    }

    [Fact]
    public void ListsRegionalSecretVersions_Empty()
    {
        string filter = "name:dotnet";

        // Get the secret name.
        SecretName secretName = _fixture.Secret.SecretName;

        // Run the code sample.
        List<SecretVersion> result = _sample.ListRegionalSecretVersionsWithFilter(
            projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId, filter: filter);

        // Assert that the result is empty.
        Assert.Empty(result);
    }
}
