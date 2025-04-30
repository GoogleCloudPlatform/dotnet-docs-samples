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

using Google.Cloud.Iam.V1;
using Google.Cloud.ParameterManager.V1;
using Google.Cloud.SecretManager.V1;

[Collection(nameof(ParameterManagerFixture))]
public class RenderParameterVersionTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly RenderParameterVersionSample _sample;

    public RenderParameterVersionTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new RenderParameterVersionSample();
    }

    [Fact]
    public void RenderParameterVersion()
    {
        string parameterId = _fixture.RandomId();
        string versionId = _fixture.RandomId();
        Parameter parameter = _fixture.CreateParameter(parameterId, ParameterFormat.Unformatted);

        Secret secret = _fixture.CreateSecret(_fixture.RandomId());
        _fixture.AddSecretVersion(secret);
        string payload = $"{{\"username\": \"test-user\", \"password\": \"__REF__(//secretmanager.googleapis.com/{secret.SecretName}/versions/latest)\"}}";
        _fixture.GrantIAMAccess(secret.SecretName, parameter.PolicyMember.IamPolicyUidPrincipal);

        ParameterVersion parameterVersion = _fixture.CreateParameterVersion(parameterId, versionId, payload);

        string result = _sample.RenderParameterVersion(projectId: _fixture.ProjectId, parameterId: parameterId, versionId);

        Assert.NotNull(result);
    }
}
