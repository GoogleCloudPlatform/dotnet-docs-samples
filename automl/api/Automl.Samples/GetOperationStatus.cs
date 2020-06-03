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

using Google.Cloud.AutoML.V1;
using Google.LongRunning;

public class AutoMLGetOperationStatus
{
    // [START automl_get_operation_status]
    /// <summary>
    /// Demonstrates using the AutoML client to get operation status.
    /// </summary>
    /// <param name="operationFullId">the complete name of a operation. For example, the name of your
    /// operation is projects/[projectId]/locations/us-central1/operations/[operationId].</param>
    public Operation GetOperationStatus(string operationFullId
        = "projects/[projectId]/locations/us-central1/operations/[operationId]")
    {
        // Initialize the client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        AutoMlClient client = AutoMlClient.Create();

        // Get the latest state of a long-running operation.
        Operation operation = client.CreateModelOperationsClient.GetOperation(operationFullId);

        // Return the result.
        return operation;
    }
    // [END automl_get_operation_status]
}
