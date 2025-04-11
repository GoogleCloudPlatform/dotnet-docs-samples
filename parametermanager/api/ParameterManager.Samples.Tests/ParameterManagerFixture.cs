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

    public string ParameterId { get; }
    public string ParameterVersionId { get; }

    public Parameter ParameterToDelete { get; }
    public ParameterName ParameterNameToDelete { get; }

    public Parameter ParameterToDeleteVersion { get; }
    public ParameterVersion ParameterVersionToDelete { get; }
    public ParameterVersionName ParameterVersionNameToDelete { get; }

    public ParameterVersion ParameterVersionToDisableAndEnable { get; }
    public ParameterVersionName ParameterVersionNameToDisableAndEnable { get; }
    public ParameterManagerFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        ParameterId = RandomId();
        ParameterToDelete = CreateParameter(ParameterId, ParameterFormat.Unformatted);
        ParameterNameToDelete = new ParameterName(ProjectId, LocationId, ParameterId);

        ParameterId = RandomId();
        ParameterVersionId = RandomId();
        ParameterToDeleteVersion = CreateParameter(ParameterId, ParameterFormat.Unformatted);
        ParameterVersionToDelete = CreateParameterVersion(ParameterId, ParameterVersionId, Payload);
        ParameterVersionNameToDelete = new ParameterVersionName(ProjectId, LocationId, ParameterId, ParameterVersionId);

        ParameterVersionId = RandomId();
        ParameterVersionToDisableAndEnable = CreateParameterVersion(ParameterId, ParameterVersionId, Payload);
        ParameterVersionNameToDisableAndEnable = new ParameterVersionName(ProjectId, LocationId, ParameterId, ParameterVersionId);
    }

    public void Dispose()
    {
        DeleteParameter(ParameterNameToDelete);
        DeleteParameterVersion(ParameterVersionNameToDelete);
        DeleteParameterVersion(ParameterVersionNameToDisableAndEnable);
        DeleteParameter(ParameterToDeleteVersion.ParameterName);
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

    public ParameterVersion CreateParameterVersion(string parameterId, string versionId, string payload)
    {
        ParameterManagerClient client = ParameterManagerClient.Create();

        ParameterName parameterName = new ParameterName(ProjectId, LocationId, parameterId);
        ParameterVersion parameterVersion = new ParameterVersion
        {
            Payload = new ParameterVersionPayload
            {
                Data = ByteString.CopyFrom(payload, Encoding.UTF8)
            }
        };

        return client.CreateParameterVersion(parameterName, parameterVersion, versionId);
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
