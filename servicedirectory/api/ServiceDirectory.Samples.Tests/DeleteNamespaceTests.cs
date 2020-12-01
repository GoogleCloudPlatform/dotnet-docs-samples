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

public class DeleteNamespaceTest : IDisposable
{ 
    private readonly ServiceDirectoryFixture _fixture;
    private readonly DeleteNamespaceSample _sample;
 
    public DeleteNamespaceTest(ServiceDirectoryFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteNamespaceSample();
    }
    
    public void Dispose() {}

    [Fact]
    public void DeletesNamespace()
    {
        // Setup namespace for the test.
        var namespaceId = _fixture.RandomResourceId();
        _fixture.CreateNamespace(namespaceId);
        // Run the sample code.
        _sample.DeleteNamespace(projectId: _fixture.ProjectId,
            locationId: _fixture.LocationId, namespaceId: namespaceId);
        
        // Try to get the namespace.
        RegistrationServiceClient registrationServiceClient = RegistrationServiceClient.Create();
        string resourceName =
            $"projects/{_fixture.ProjectId}/locations/{_fixture.LocationId}/namespaces/{namespaceId}";
        try
        {
            registrationServiceClient.GetNamespace(resourceName);
        }
        catch (Grpc.Core.RpcException exception)
        {
            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }
    }
}