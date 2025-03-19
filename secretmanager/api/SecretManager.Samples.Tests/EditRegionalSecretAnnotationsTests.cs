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
public class EditRegionalSecretAnnotationsTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly EditRegionalSecretAnnotationsSample _sample;

    public EditRegionalSecretAnnotationsTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new EditRegionalSecretAnnotationsSample();
    }

    [Fact]
    public void EditRegionalSecretsAnnotations()
    {
        // Get the SecretName from the set ProjectId & LocationId.
        SecretName secretName = _fixture.Secret.SecretName;

        // New annotation key value.
        string newAnnotationKey = "new-annotation-key";
        string newAnnotationValue = "new-annotation-value";

        // Existing annotation key value from the fixture class.
        string oldAnnotationKey = _fixture.AnnotationKey;
        string oldAnnotationValue = _fixture.AnnotationValue;

        // Run the code sample.
        Secret result = _sample.EditRegionalSecretAnnotations(
          projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId, annotationKey: newAnnotationKey, annotationValue: newAnnotationValue);

        // Assert that secretId matches with the expected value.
        Assert.Equal(result.SecretName.SecretId, secretName.SecretId);

        // Assert that the new key-value matches with the expected value.
        Assert.Equal(result.Annotations[newAnnotationKey], newAnnotationValue);

        // Assert that the old key-value matches with the expected value.
        Assert.Equal(result.Annotations[oldAnnotationKey], oldAnnotationValue);
    }
}
