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
using System.Linq;

[Collection(nameof(SecretManagerFixture))]
public class ListSecretsWithFilterTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly ListSecretsWithFilterSample _sample;

    public ListSecretsWithFilterTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListSecretsWithFilterSample();
    }

    [Fact]
    public void ListsSecretsWithFilter()
    {
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());
        SecretName secretName = secret.SecretName;


        IList<Secret> secrets = _sample.ListSecretsWithFilter(
                    projectId: _fixture.ProjectId);

        // Verify we got results
        Assert.NotNull(secrets);
        Assert.NotEmpty(secrets);

        // Verify our specific secret is in the results
        bool foundSecret = secrets.Any(s => s.SecretName.SecretId == secretName.SecretId);
        Assert.True(foundSecret, $"The secret {secretName.SecretId} with label my-label-key=my-label-value should be in the results");

        _fixture.DeleteSecret(secretName);
    }
}
