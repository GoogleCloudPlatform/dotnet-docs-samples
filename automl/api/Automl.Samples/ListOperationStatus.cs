// Copyright (c) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START automl_list_operation_status]

using System;
using System.Linq;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.AutoML.V1;
using Google.LongRunning;
using System.Collections.Generic;

public class AutoMLListOperationStatus
{
    /// <summary>
    /// Demonstrates using the AutoML client to list operations.
    /// </summary>
    /// <param name="projectId">GCP Project ID.</param>
    /// <param name="location">Region.</param>
    public IEnumerable<Operation> ListOperationStatus(string projectId = "YOUR-PROJECT-ID", string location = "YOUR-REGION")
    {
        // Initialize the client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        AutoMlClient client = AutoMlClient.Create();

        // A resource that represents Google Cloud Platform location.
        LocationName locationName = new LocationName(projectId, location);

        // Create list operations request.
        ListOperationsRequest listrequest = new ListOperationsRequest
        {
            Name = locationName.ToString()
        };

        // Call the API.
        IEnumerable<Operation> operations = client.CreateModelOperationsClient.ListOperations(listrequest);

        Operation firstOperation = operations.First();
        Console.WriteLine("Operation details:");
        Console.WriteLine($"\tName: {firstOperation.Name}");
        Console.WriteLine($"\tDone: {firstOperation.Done}");

        // Return the result.
        return operations;
    }
}
// [END automl_list_operation_status]