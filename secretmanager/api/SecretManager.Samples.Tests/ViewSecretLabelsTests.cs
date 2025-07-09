/*
 * Copyright 2025 Google LLC
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
using Xunit;


[Collection(nameof(SecretManagerFixture))]
public class ViewSecretLabelsTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly ViewSecretLabelsSample _sample;

    public ViewSecretLabelsTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ViewSecretLabelsSample();
    }

    [Fact]
    public void ViewSecretsLabels()
    {
        // Get the SecretName from the set ProjectId.
        SecretName secretName = _fixture.Secret.SecretName;

        // Label Key-Value from the fixture class.
        string labelKey = _fixture.LabelKey;
        string labelValue = _fixture.LabelValue;

        // Run the code sample.
        Secret result = _sample.ViewSecretLabels(
          projectId: secretName.ProjectId, secretId: secretName.SecretId);

        // Assert that the secretId is equal to the expected value.
        Assert.Equal(result.SecretName.SecretId, secretName.SecretId);

        // Assert that the label key's value matches with the expected value.
        Assert.Equal(result.Labels[labelKey], labelValue);
    }
}
