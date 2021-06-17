/*
 * Copyright 2021 Google LLC
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

// [START compute_instances_operation_check]

using Google.Cloud.Compute.V1;
using System;
using System.Threading.Tasks;

public static class OperationPoller
{
    public static async Task<Operation> PollUntilCompletedAsync(this Operation operation, string projectId, string zone)
    {
        ZoneOperationsClient operationClient = await ZoneOperationsClient.CreateAsync();
        GetZoneOperationRequest pollRequest = new GetZoneOperationRequest
        {
            Operation = operation.Name,
            Zone = zone,
            Project = projectId,
        };

        TimeSpan timeOut = TimeSpan.FromMinutes(3);
        TimeSpan pollInterval = TimeSpan.FromSeconds(15);

        DateTime deadline = DateTime.UtcNow + timeOut;
        while (operation.Status != Operation.Types.Status.Done)
        {
            operation = await operationClient.GetAsync(pollRequest);

            if (operation.Status == Operation.Types.Status.Done)
            {
                break;
            }

            if (DateTime.UtcNow + pollInterval > deadline)
            {
                throw new InvalidOperationException("Timeout hit while polling for the status of the operation.");
            }

            await Task.Delay(pollInterval);
        }

        return operation;
    }
}

// [END compute_instances_operation_check]
