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
        // Get the secret name.
        SecretName secretName = _fixture.Secret.SecretName;

        // Run the code sample.
        List<SecretVersion> result = _sample.ListRegionalSecretVersions(
            projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId);

        // Assert that the secret list is not empty.
        Assert.NotEmpty(result);
    }
}
