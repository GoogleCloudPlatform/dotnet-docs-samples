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

[CollectionDefinition(nameof(ParameterManagerRegionalFixture))]
public class ParameterManagerRegionalFixture : IDisposable, ICollectionFixture<ParameterManagerRegionalFixture>
{
    public string ProjectId { get; }
    public const string LocationId = "us-central1";

    public ParameterManagerClient Client { get; }
    internal List<ParameterName> ParametersToDelete { get; } = new List<ParameterName>();
    internal List<ParameterVersionName> ParameterVersionsToDelete { get; } = new List<ParameterVersionName>();

    public ParameterManagerRegionalFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        // Define the regional endpoint
        string regionalEndpoint = $"parametermanager.{LocationId}.rep.googleapis.com";

        // Create the client with the regional endpoint
        Client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();
    }

    public void Dispose()
    {
        foreach (var parameterVersion in ParameterVersionsToDelete)
        {
            DeleteParameterVersion(parameterVersion);
        }
        foreach (var parameter in ParametersToDelete)
        {
            DeleteParameter(parameter);
        }
    }

    public string RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }

    public (string parameterId, string versionId, Parameter parameter, string payload) CreateParameterWithVersion()
    {
        string parameterId = RandomId();
        string versionId = RandomId();
        Parameter parameter = CreateParameter(parameterId, ParameterFormat.Unformatted);
        string payload = "test123";
        return (parameterId, versionId, parameter, payload);
    }

    public Parameter CreateParameter(string parameterId, ParameterFormat format)
    {
        LocationName parent = new LocationName(ProjectId, LocationId);

        Parameter parameter = new Parameter
        {
            Format = format
        };

        Parameter Parameter = Client.CreateParameter(parent, parameter, parameterId);
        ParametersToDelete.Add(Parameter.ParameterName);
        return Parameter;
    }

    public ParameterVersion CreateParameterVersion(string parameterId, string versionId, string payload)
    {
        ParameterName parameterName = new ParameterName(ProjectId, LocationId, parameterId);
        ParameterVersion parameterVersion = new ParameterVersion
        {
            Payload = new ParameterVersionPayload
            {
                Data = ByteString.CopyFrom(payload, Encoding.UTF8)
            }
        };

        ParameterVersion ParameterVersion = Client.CreateParameterVersion(parameterName, parameterVersion, versionId);
        ParameterVersionsToDelete.Add(ParameterVersion.ParameterVersionName);
        return ParameterVersion;
    }

    private void DeleteParameter(ParameterName name)
    {
        try
        {
            Client.DeleteParameter(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - Parameter was already deleted
        }
    }

    private void DeleteParameterVersion(ParameterVersionName name)
    {
        try
        {
            Client.DeleteParameterVersion(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - Parameter version was already deleted
        }
    }
}
