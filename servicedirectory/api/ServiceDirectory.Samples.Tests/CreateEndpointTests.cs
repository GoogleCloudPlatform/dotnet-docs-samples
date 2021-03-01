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
using Xunit;

[Collection(nameof(ServiceDirectoryFixture))]
public class CreateEndpointTest
{ 
    private readonly ServiceDirectoryFixture _fixture;

    public CreateEndpointTest(ServiceDirectoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CreatesEndpoint()
    {
        // Setup namespace and service for test
        var namespaceId = _fixture.RandomResourceId;
        var serviceId = _fixture.RandomResourceId;
        _fixture.CreateNamespace(namespaceId);
        _fixture.CreateService(namespaceId, serviceId);

        var endpointId = _fixture.RandomResourceId;
        var createEndpointSample = new CreateEndpointSample();

        var result = createEndpointSample.CreateEndpoint(_fixture.ProjectId, _fixture.LocationId, namespaceId, serviceId, endpointId);

        var endpointName = EndpointName.FromProjectLocationNamespaceServiceEndpoint(_fixture.ProjectId, _fixture.LocationId, namespaceId, serviceId, endpointId);
        RegistrationServiceClient registrationServiceClient = RegistrationServiceClient.Create();
        var endpoint = registrationServiceClient.GetEndpoint(endpointName);
        
        Assert.Equal(endpoint.Name, result.Name);
    }
}