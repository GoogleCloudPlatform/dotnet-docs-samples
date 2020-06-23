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
    /// <param name="location"> Region.</param>
    /// <param name="operationId">Operation ID. </param>
    public Operation<Model, OperationMetadata> GetOperationStatus(
        string projectId = "YOUR-PROJECT-ID", string location = "YOUR-REGION",
        string operationId = "YOUR-OPERATION-ID")
    {
        // Initialize the client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        AutoMlClient client = AutoMlClient.Create();
        string operationFullId = $"projects/{projectId}/locations/{location}/operations/{operationId}";

        // Get the latest state of a long-running operation.
        // This will only work for creating model operation, and for other statuses, the method has to be
        // changed. For example, client.PollOnceCreateDataset for checking the status of creating a dataset.
        Operation<Model, OperationMetadata> operation = client.PollOnceCreateModel(operationFullId);

        // Display operation details.
        Console.WriteLine("Operation details:");
        Console.WriteLine($"Name: {operation.Name}");
        Console.WriteLine($"Done: {operation.IsCompleted}");
        // Return the result.
        return operation;
    }
}
// [END automl_get_operation_status]