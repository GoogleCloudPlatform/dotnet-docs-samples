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


[Collection(nameof(RegionalSecretManagerFixture))]
public class CreateRegionalSecretWithAnnotationsTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly CreateRegionalSecretWithAnnotationsSample _sample;

    public CreateRegionalSecretWithAnnotationsTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateRegionalSecretWithAnnotationsSample();
    }

    [Fact]
    public void CreatesRegionalSecretsWithAnnotations()
    {
        // Get the SecretName from the set ProjectId & LocationId.
        SecretName secretName = SecretName.FromProjectLocationSecret(_fixture.ProjectId, _fixture.LocationId, _fixture.RandomId());

        // Get the annotation key & value from the fixture class.
        string annotationKey = _fixture.AnnotationKey;
        string annotationValue = _fixture.AnnotationValue;

        // Run the code sample.
        Secret result = _sample.CreateRegionalSecretWithAnnotations(
          projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId, annotationKey: annotationKey, annotationValue: annotationValue);

        // Assert that the secretId is same as expected.
        Assert.Equal(result.SecretName.SecretId, secretName.SecretId);

        // Assert that the annotation values matches with the expected value.
        Assert.Equal(result.Annotations[annotationKey], annotationValue);

        // Clean the created secret.
        _fixture.DeleteSecret(secretName);
    }
}
