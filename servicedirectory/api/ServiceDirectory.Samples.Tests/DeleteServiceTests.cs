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
using Grpc.Core;
using Xunit;

[Collection(nameof(ServiceDirectoryFixture))]

public class DeleteServiceTest
{ 
    private readonly ServiceDirectoryFixture _fixture;
    private readonly DeleteServiceSample _sample;
    private string _namespaceId;
 
    public DeleteServiceTest(ServiceDirectoryFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteServiceSample();
    }

    [Fact]
    public void DeletesService()
    {
        // Setup namespace and service for the test.
        _namespaceId = _fixture.RandomResourceId();
        var serviceId = _fixture.RandomResourceId();
        _fixture.CreateNamespace(_namespaceId);
        _fixture.CreateService(_namespaceId, serviceId);
        // Run the sample code.
        _sample.DeleteService(_fixture.ProjectId, _fixture.LocationId, _namespaceId, serviceId);
        
        // Try to get the service.
        RegistrationServiceClient registrationServiceClient = RegistrationServiceClient.Create();
        var serviceName = ServiceName.FromProjectLocationNamespaceService(_fixture.ProjectId, _fixture.LocationId, _namespaceId,
                serviceId);
        var exception = Assert.Throws<RpcException>(() => registrationServiceClient.GetService(serviceName));
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }
}