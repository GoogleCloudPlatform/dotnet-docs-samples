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
public class CreateSecretWithAnnotationsTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly CreateSecretWithAnnotationsSample _sample;

    public CreateSecretWithAnnotationsTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateSecretWithAnnotationsSample();
    }

    [Fact]
    public void CreatesSecretsWithAnnotations()
    {
        // Get the SecretName to create Secret.
        SecretName secretName = new SecretName(_fixture.ProjectId, _fixture.RandomId());

        // Get the annotation key & value from the fixture class.
        string annotationKey = _fixture.AnnotationKey;
        string annotationValue = _fixture.AnnotationValue;

        // Create the secret with the specificed fields.
        Secret result = _sample.CreateSecretWithAnnotations(
          projectId: secretName.ProjectId, secretId: secretName.SecretId, annotationKey: annotationKey, annotationValue: annotationValue);

        // Assert that the secretId is same as expected.
        Assert.Equal(result.SecretName.SecretId, secretName.SecretId);

        // Assert that the annotation values matches with the expected value.
        Assert.Equal(result.Annotations[annotationKey], annotationValue);

        // Clean the created secret.
        _fixture.DeleteSecret(secretName);
    }
}
