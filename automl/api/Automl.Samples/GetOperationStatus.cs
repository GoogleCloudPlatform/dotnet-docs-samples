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

// [START automl_get_operation_status]

using Google.Cloud.AutoML.V1;
using Google.LongRunning;
using System;

public class AutoMLGetOperationStatus
{
    /// <summary>
    /// Demonstrates using the AutoML client to get operation status.
    /// </summary>
    /// <param name="projectId">GCP Project ID.</param>
    /// <param name="location">GCP Project Region.</param>
    /// <param name="operationId">Operation ID. </param>
    public Operation GetOperationStatus(string projectId = "YOUR-PROJECT-ID",
        string location = "YOUR-PROJECT-LOCATION", string operationId
        = "YOUR-OPERATION-ID")
    {
        // Initialize the client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        AutoMlClient client = AutoMlClient.Create();
        string operationFullId = $"projects/{projectId}/locations/{location}/operations/{operationId}";

        // Get the latest state of a long-running operation.
        Operation operation = client.CreateModelOperationsClient.GetOperation(operationFullId);


        // Display operation details.
        Console.WriteLine("Operation details:");
        Console.WriteLine($"Name: {operation.Name}");
        Console.WriteLine($"Metadata Type Url: {operation.Metadata.TypeUrl}");
        Console.WriteLine($"Done: {operation.Done}");
        if (operation.Response != null)
        {
            Console.WriteLine($"Response Type Url: {operation.Response.TypeUrl}");
        }
        if (operation.Error != null)
        {
            Console.WriteLine("\tResponse:");
            Console.WriteLine($"\t\tError code: {operation.Error.Code}");
            Console.WriteLine($"\t\tError message: {operation.Error.Message}");
        }
        // Return the result.
        return operation;
    }
}
// [END automl_get_operation_status]