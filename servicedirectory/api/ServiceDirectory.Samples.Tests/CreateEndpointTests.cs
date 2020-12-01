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
 
using Google.Cloud.ServiceDirectory.V1Beta1;
using System;
using Xunit;

[Collection(nameof(ServiceDirectoryFixture))]

public class CreateEndpointTest : IDisposable
{ 
    private readonly ServiceDirectoryFixture _fixture;
    private readonly CreateEndpointSample _sample;
    private string _namespaceId;

    public CreateEndpointTest(ServiceDirectoryFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateEndpointSample();
    }

    public void Dispose()
    {
        _fixture.DeleteNamespace(_namespaceId);
    }

    [Fact]
    public void CreatesEndpoint()
    {
        // Setup namespace and service for test
        _namespaceId = _fixture.RandomResourceId();
        var serviceId = _fixture.RandomResourceId();
        _fixture.CreateNamespace(_namespaceId);
        _fixture.CreateService(_namespaceId, serviceId);
        
        var endpointId = _fixture.RandomResourceId();
        // Run the sample code.
        var result = _sample.CreateEndpoint(projectId: _fixture.ProjectId,
            locationId: _fixture.LocationId, namespaceId: _namespaceId, serviceId: serviceId,
            endpointId: endpointId);

        // Get the endpoint.
        string resourceName =
            $"projects/{_fixture.ProjectId}/locations/{_fixture.LocationId}/namespaces/{_namespaceId}/services/{serviceId}/endpoints/{endpointId}";
        RegistrationServiceClient registrationServiceClient = RegistrationServiceClient.Create();
        var endpoint = registrationServiceClient.GetEndpoint(resourceName);

        Assert.Contains(endpointId, endpoint.Name);
    }
}