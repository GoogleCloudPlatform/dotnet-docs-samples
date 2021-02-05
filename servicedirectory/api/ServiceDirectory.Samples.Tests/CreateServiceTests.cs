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

public class CreateServiceTest
{
    private readonly ServiceDirectoryFixture _fixture;

    public CreateServiceTest(ServiceDirectoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CreatesService()
    {
        // Setup namespace for the test.
        var namespaceId = _fixture.RandomResourceId;
        _fixture.CreateNamespace(namespaceId);
        
        var serviceId = _fixture.RandomResourceId;
        // Run the sample code.
        var createServiceSample = new CreateServiceSample();
        var result = createServiceSample.CreateService(_fixture.ProjectId, _fixture.LocationId, namespaceId, serviceId);

        // Get the service.
        var serviceName = ServiceName.FromProjectLocationNamespaceService(_fixture.ProjectId, _fixture.LocationId, namespaceId, serviceId);
        RegistrationServiceClient registrationServiceClient = RegistrationServiceClient.Create();
        var service = registrationServiceClient.GetService(serviceName);

        Assert.Contains(serviceId, service.Name);
    }
}