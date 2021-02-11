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

// [START servicedirectory_quickstart]

using Google.Cloud.ServiceDirectory.V1;

public class QuickstartSample
{
	public void Quickstart(
        string projectId = "my-project",
        string locationId = "us-east1",
        string namespaceId = "test-namespace",
        string serviceId = "test-service",
        string endpointId = "test-endpoint")
    {
        // Create registration client
        RegistrationServiceClient registrationServiceClient = RegistrationServiceClient.Create();

        var locationName = LocationName.FromProjectLocation(projectId, locationId);
        registrationServiceClient.CreateNamespace(locationName, new Namespace(), namespaceId);
        
        var namespaceName = NamespaceName.FromProjectLocationNamespace(projectId, locationId, namespaceId);
        registrationServiceClient.CreateService(namespaceName, new Service(), serviceId);
        
        var serviceName = ServiceName.FromProjectLocationNamespaceService(projectId, locationId, namespaceId, serviceId);
        registrationServiceClient.CreateEndpoint(serviceName, new Endpoint(), endpointId);
        
        // Create lookup client
        LookupServiceClient lookupServiceClient = LookupServiceClient.Create();
        ResolveServiceRequest request = new ResolveServiceRequest
        {
            ServiceName = ServiceName.FromProjectLocationNamespaceService(projectId, locationId, namespaceId, serviceId),
        };
        // Make the resolve request
        ResolveServiceResponse response = lookupServiceClient.ResolveService(request);
        
        // Delete Namespace
        registrationServiceClient.DeleteNamespace(namespaceName);
    }
}

// [END servicedirectory_quickstart]
