/*
 * Copyright 2020 Google LLC
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
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class ListRegionalSecretsWithFilterTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly ListRegionalSecretsWithFilterSample _sample;

    public ListRegionalSecretsWithFilterTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListRegionalSecretsWithFilterSample();
    }

    [Fact]
    public void ListsRegionalSecretsWithFilter()
    {
        SecretName secretName = _fixture.Secret.SecretName;

        string filter = "name:my-secret";

        _sample.ListRegionalSecretsWithFilter(projectId: secretName.ProjectId, locationId: secretName.LocationId, filter: filter);
    }
}
