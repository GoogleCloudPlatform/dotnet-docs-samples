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

// [START servicedirectory_create_endpoint]

using Google.Cloud.ServiceDirectory.V1Beta1;

public class CreateEndpointSample
{
	public Endpoint CreateEndpoint(string projectId = "my-project",
        string locationId = "us-east1",
        string namespaceId = "test-namespace",
        string serviceId = "test-service",
        string endpointId = "test-endpoint")
    {
        RegistrationServiceClient registrationServiceClient = RegistrationServiceClient.Create();
        CreateEndpointRequest request = new CreateEndpointRequest
        {
                ParentAsServiceName = ServiceName.FromProjectLocationNamespaceService(projectId, locationId, namespaceId, serviceId),
                EndpointId = endpointId,
                Endpoint = new Endpoint(),
        };
        // Make the request
        return registrationServiceClient.CreateEndpoint(request);
        // End snippet
    }
}

// [END servicedirectory_create_endpoint]