// Copyright(c) 2025 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Iam.V1;
using Google.Cloud.ParameterManager.V1;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf;
using System.Text;

[CollectionDefinition(nameof(ParameterManagerFixture))]
public class ParameterManagerFixture : IDisposable, ICollectionFixture<ParameterManagerFixture>
{
    public string ProjectId { get; }
    public const string LocationId = "global";

    public const string Payload = "test123";
    public const string JsonPayload = "{\"username\": \"test-user\", \"host\": \"localhost\"}";
    public const string SecretId = "projects/project-id/secrets/secret-id/versions/latest";

    public ParameterName ParameterName { get; }
    public ParameterName ParameterNameWithFormat { get; }

    public string ParameterId { get; }
    public Parameter Parameter { get; }
    public ParameterVersionName ParameterVersionName { get; }

    public Parameter ParameterWithFormat { get; }
    public ParameterVersionName ParameterVersionNameWithFormat { get; }

    public Parameter ParameterWithSecretReference { get; }
    public ParameterVersionName ParameterVersionNameWithSecretReference { get; }

    public ParameterManagerFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        ParameterName = new ParameterName(ProjectId, LocationId, RandomId());
        ParameterNameWithFormat = new ParameterName(ProjectId, LocationId, RandomId());

        ParameterId = RandomId();
        Parameter = CreateParameter(ParameterId, ParameterFormat.Unformatted);
        ParameterVersionName = new ParameterVersionName(ProjectId, LocationId, ParameterId, RandomId());

        ParameterId = RandomId();
        ParameterWithFormat = CreateParameter(ParameterId, ParameterFormat.Json);
        ParameterVersionNameWithFormat = new ParameterVersionName(ProjectId, LocationId, ParameterId, RandomId());

        ParameterId = RandomId();
        ParameterWithSecretReference = CreateParameter(ParameterId, ParameterFormat.Unformatted);
        ParameterVersionNameWithSecretReference = new ParameterVersionName(ProjectId, LocationId, ParameterId, RandomId());
    }

    public void Dispose()
    {
        DeleteParameter(ParameterName);
        DeleteParameter(ParameterNameWithFormat);
        DeleteParameterVersion(ParameterVersionName);
        DeleteParameter(Parameter.ParameterName);
        DeleteParameterVersion(ParameterVersionNameWithFormat);
        DeleteParameter(ParameterWithFormat.ParameterName);
        DeleteParameterVersion(ParameterVersionNameWithSecretReference);
        DeleteParameter(ParameterWithSecretReference.ParameterName);
    }

    public String RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }

    public Parameter CreateParameter(string parameterId, ParameterFormat format)
    {
        ParameterManagerClient client = ParameterManagerClient.Create();
        LocationName projectName = new LocationName(ProjectId, LocationId);

        Parameter parameter = new Parameter
        {
            Format = format
        };

        return client.CreateParameter(projectName, parameter, parameterId);
    }

    private void DeleteParameter(ParameterName name)
    {
        ParameterManagerClient client = ParameterManagerClient.Create();
        try
        {
            client.DeleteParameter(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - Parameter was already deleted
        }
    }

    private void DeleteParameterVersion(ParameterVersionName name)
    {
        ParameterManagerClient client = ParameterManagerClient.Create();
        try
        {
            client.DeleteParameterVersion(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - Parameter version was already deleted
        }
    }
}
