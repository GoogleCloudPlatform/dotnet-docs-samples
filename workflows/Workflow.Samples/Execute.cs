/*
 * Copyright 2025 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https:*www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START workflows_api_quickstart]

using System;
using System.Threading.Tasks;

using Google.Cloud.Workflows.Executions.V1;
using Google.Cloud.Workflows.Common.V1;

public class ExecuteWorkflowSample
{
    /// <summary>
    /// Execute a workflow and return the execution operation.
    ///</summary>
    /// <param name="projectId">Your Google Cloud Project ID.</param>
    /// <param name="locationID">The region where your workflow is located</param>
    /// <param name="workflowID">Your Google Cloud Workflow ID.</param>
    /// 
    public async Task<Execution> ExecuteWorkflow(
        string projectId = "YOUR-PROJECT-ID",
        string locationID = "YOUR-LOCATION-ID",
        string workflowID = "YOUR-WORKFLOW-ID"
    )
    {
        // Arbitrary values
        int delay = 1000;
        TimeSpan timeout = TimeSpan.FromMinutes(5);

        // Initialize the client.
        ExecutionsClient client = await ExecutionsClient.CreateAsync();

        // Build the parent location path.
        string parent = WorkflowName.Format(projectId, locationID, workflowID);

        // Craete te execution request.
        CreateExecutionRequest req = new CreateExecutionRequest
        {
            ParentAsWorkflowName = WorkflowName.Parse(parent),
        };

        // Execute the operation.. 
        Execution execution = await client.CreateExecutionAsync(req);
        Console.WriteLine("- Execution started...");

        Execution fetchedExecution;
        DateTime startTime = DateTime.UtcNow;

        // Loop to check whether the execution is done or the timeout has been reached.
        do
        {
            fetchedExecution = await client.GetExecutionAsync(execution.Name);

            if (DateTime.UtcNow - startTime > timeout)
            {
                Console.WriteLine($"Timeout reached after {timeout}");
                break;
            }
            else
            {
                Console.WriteLine("- Waiting for results...");

                await Task.Delay(delay);
                delay *= 2;
            }

        } while (fetchedExecution.State == Execution.Types.State.Active);

        return fetchedExecution;
    }
}
// [END workflows_api_quickstart]