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

using Google.Cloud.ServiceDirectory.V1;
using System;
using System.Collections.Generic;
using Xunit;

[CollectionDefinition(nameof(ServiceDirectoryFixture))]
public class ServiceDirectoryFixture : IDisposable, ICollectionFixture<ServiceDirectoryFixture>
{
    public string ProjectId { get; }

    public string LocationId { get; } = "us-east1";
    
    public IList<string> TempNamespaceIds { get; } = new List<string>();

    public ServiceDirectoryFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }
    }
    
    public string RandomResourceId => $"csharp-{Guid.NewGuid()}";

    public void Dispose()
    {
        var registrationServiceClient = RegistrationServiceClient.Create();
        foreach (var namespaceId in TempNamespaceIds)
        {
            try
            {
                var namespaceName = NamespaceName.FromProjectLocationNamespace(ProjectId, LocationId, namespaceId);
                registrationServiceClient.DeleteNamespace(namespaceName);
            }
            catch (Exception)
            {
                // Do nothing, we delete on a best effort basis.
            }
        }
    }

    public void CreateNamespace(string namespaceId)
    {
        var createNamespaceSample = new CreateNamespaceSample();
        createNamespaceSample.CreateNamespace(ProjectId, LocationId, namespaceId);
        TempNamespaceIds.Add(namespaceId);
    }

    public void CreateService(string namespaceId, string serviceId)
    {
        var createServiceSample = new CreateServiceSample();
        createServiceSample.CreateService(ProjectId, LocationId, namespaceId, serviceId);
    }

    public void CreateEndpoint(string namespaceId, string serviceId, string endpointId)
    {
        var createEndpointSample = new CreateEndpointSample();
        createEndpointSample.CreateEndpoint(ProjectId, LocationId, namespaceId, serviceId, endpointId);
    }
}