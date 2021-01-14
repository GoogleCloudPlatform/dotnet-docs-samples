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
using Grpc.Core;
using Xunit;

[Collection(nameof(ServiceDirectoryFixture))]

public class DeleteEndpointTest
{ 
    private readonly ServiceDirectoryFixture _fixture;

    public DeleteEndpointTest(ServiceDirectoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void DeletesEndpoint()
    {
        // Setup namespace, service, and endpoint for the test.
        var namespaceId = _fixture.RandomResourceId;
        var serviceId = _fixture.RandomResourceId;
        var endpointId = _fixture.RandomResourceId;
        _fixture.CreateNamespace(namespaceId);
        _fixture.CreateService(namespaceId, serviceId);
        _fixture.CreateEndpoint(namespaceId, serviceId, endpointId);
        // Run the sample code.
        var deleteEndpointSample = new DeleteEndpointSample();
        deleteEndpointSample.DeleteEndpoint(_fixture.ProjectId, _fixture.LocationId, namespaceId, serviceId, endpointId);
        
        // Try to get the endpoint.
        RegistrationServiceClient registrationServiceClient = RegistrationServiceClient.Create();
        var endpointName = EndpointName.FromProjectLocationNamespaceServiceEndpoint(_fixture.ProjectId, _fixture.LocationId, namespaceId, serviceId, endpointId);
        var exception = Assert.Throws<RpcException>(() => registrationServiceClient.GetEndpoint(endpointName));
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }
}