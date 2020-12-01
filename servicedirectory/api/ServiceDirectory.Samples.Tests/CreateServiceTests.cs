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

public class CreateServiceTest : IDisposable
{
    private readonly ServiceDirectoryFixture _fixture;
    private readonly CreateServiceSample _sample;
    private string _namespaceId;

    public CreateServiceTest(ServiceDirectoryFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateServiceSample();
    }

    public void Dispose()
    {
        _fixture.DeleteNamespace(_namespaceId);
    }

    [Fact]
    public void CreatesEndpoint()
    {
        // Setup namespace for the test.
        _namespaceId = _fixture.RandomResourceId();
        _fixture.CreateNamespace(_namespaceId);

        // Run the sample code.
        var serviceId = _fixture.RandomResourceId();
        var result = _sample.CreateService(projectId: _fixture.ProjectId,
            locationId: _fixture.LocationId, namespaceId: _namespaceId, serviceId: serviceId);

        // Get the service.
        string resourceName =
            $"projects/{_fixture.ProjectId}/locations/{_fixture.LocationId}/namespaces/{_namespaceId}/services/{serviceId}";
        RegistrationServiceClient registrationServiceClient = RegistrationServiceClient.Create();
        var service = registrationServiceClient.GetService(resourceName);

        Assert.Contains(serviceId, service.Name);
    }
}