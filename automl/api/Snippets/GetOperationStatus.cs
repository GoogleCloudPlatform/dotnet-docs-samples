// Copyright (c) 2019 Google LLC.
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

class AutoMLGetOperationStatus
{
    /// <summary>
    /// Demonstrates using the AutoML client to get operation status.
    /// </summary>
    /// <param name="operationFullId">the complete name of a operation. For example, the name of your
    /// operation is projects/[projectId]/locations/us-central1/operations/[operationId].</param>
    public static void GetOperationStatus(string operationFullId
        = "projects/[projectId]/locations/us-central1/operations/[operationId]")
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests. After completing all of your requests, call
        // the "close" method on the client to safely clean up any remaining background resources.
        AutoMlClient client = AutoMlClient.Create();

        // Get the latest state of a long-running operation.
        //TODO: I dont know why there is no 'GetOperationsClient'
        Operation operation = client.CreateModelOperationsClient.GetOperation(operationFullId);

        // Display operation details.
        Console.WriteLine("Operation details:");
        Console.WriteLine($"\tName: {operation.Name}");
        Console.WriteLine($"\tMetadata Type Url: {operation.Metadata.TypeUrl}");
        Console.WriteLine($"\tDone: {operation.Done}");
        if (operation.Response != null)
        {
            Console.WriteLine($"\tResponse Type Url: {operation.Response.TypeUrl}");
        }
        if (operation.Error != null)
        {
            Console.WriteLine("\tResponse:");
            Console.WriteLine($"\t\tError code: {operation.Error.Code}");
            Console.WriteLine($"\t\tError message: {operation.Error.Message}");
        }
    }
}

// [END automl_get_operation_status]