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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ServiceDirectory.V1Beta1;
using System;
using Xunit;

[CollectionDefinition(nameof(ServiceDirectoryFixture))]
public class ServiceDirectoryFixture : IDisposable, ICollectionFixture<ServiceDirectoryFixture>
{
    public string ProjectId { get; }
    public ProjectName ProjectName { get; }
    
    public string LocationId { get; }
    public LocationName LocationName { get; }

    public ServiceDirectoryFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }
        ProjectName = new ProjectName(ProjectId);

        LocationId = "us-east1";
        LocationName = new LocationName(ProjectId, LocationId);
    }
    
    public string RandomResourceId()
    {
        return $"csharp-{Guid.NewGuid()}";
    }
    
    public void Dispose(){}

    public void CreateNamespace(string namespaceId)
    {
        var registrationServiceClient = RegistrationServiceClient.Create();
        var request = new CreateNamespaceRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(ProjectId, LocationId),
            NamespaceId = namespaceId,
            Namespace = new Namespace(),
        };
        registrationServiceClient.CreateNamespace(request);
    }

    public void CreateService(string namespaceId, string serviceId)
    {
        var registrationServiceClient = RegistrationServiceClient.Create();
        var request = new CreateServiceRequest
        {
            ParentAsNamespaceName = NamespaceName.FromProjectLocationNamespace(ProjectId, LocationId, namespaceId),
            ServiceId = serviceId,
            Service = new Service(),
        };
        registrationServiceClient.CreateService(request);
    }

    public void CreateEndpoint(string namespaceId, string serviceId, string endpointId)
    {
        var registrationServiceClient = RegistrationServiceClient.Create();
        var request = new CreateEndpointRequest
        {
            ParentAsServiceName = ServiceName.FromProjectLocationNamespaceService(ProjectId, LocationId, namespaceId, serviceId),
            EndpointId = endpointId,
            Endpoint = new Endpoint(),
        };
        registrationServiceClient.CreateEndpoint(request);
    }

    public void DeleteNamespace(string namespaceId)
    {
        var registrationServiceClient = RegistrationServiceClient.Create();
        var request = new DeleteNamespaceRequest
        {
            NamespaceName = NamespaceName.FromProjectLocationNamespace(ProjectId, LocationId, namespaceId),
        };
        registrationServiceClient.DeleteNamespace(request);
    }
}