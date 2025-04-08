// Copyright(c) 2025 Google Inc.
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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Workflows.Common.V1;
using Google.Cloud.Workflows.V1;
using Google.LongRunning;

[CollectionDefinition(nameof(WorkflowFixture))]
public class WorkflowFixture : IDisposable, ICollectionFixture<WorkflowFixture>
{
    public string LocationId { get; } = "us-central1";
    public string ProjectId { get; }
    public string WorkflowID { get; }
    public Workflow Workflow { get; }

    public WorkflowFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        // Generate a random ID for the testing workflow.
        WorkflowID = GetRandomWorkflowId();

        // Create workflow with the given ID.
        Workflow = CreateWorkflow(WorkflowID);

    }

    /// <summary>
    /// Create a workflow by the given ID and return it.
    /// </summary>
    /// <param name="workflowID">The workflow's ID</param>
    /// 
    public Workflow CreateWorkflow(string workflowID)
    {
        WorkflowsClient client = WorkflowsClient.Create();

        string parent = LocationName.Format(ProjectId, LocationId);
        string filePath = Path.Combine(AppContext.BaseDirectory, "myFirstWorkflow.yaml");
        string fileContent = File.ReadAllText(filePath);

        Workflow workflow = new Workflow
        {
            Name = WorkflowName.Format(ProjectId, LocationId, workflowID),
            SourceContents = fileContent
        };

        CreateWorkflowRequest createWorkflowReq = new CreateWorkflowRequest
        {
            Parent = parent,
            Workflow = workflow,
            WorkflowId = workflowID
        };


        Operation<Workflow, OperationMetadata> operation = client.CreateWorkflow(createWorkflowReq);
        Operation<Workflow, OperationMetadata> deployedWorkflow = operation.PollUntilCompletedAsync().GetAwaiter().GetResult();

        // Get the deployed workflow once 
        return deployedWorkflow.Result;
    }

    /// <summary>
    /// Delete a workflow by the given workflow name and return it.
    /// </summary>
    /// <param name="workflowName">The workflow's ID</param>
    /// 
    public void DeleteWorkflow(string workflowName)
    {
        WorkflowsClient client = WorkflowsClient.Create();

        DeleteWorkflowRequest deleteWorkflowReq = new DeleteWorkflowRequest
        {
            Name = workflowName,
        };

        try
        {
            client.DeleteWorkflow(deleteWorkflowReq);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - secret was already deleted.
        }
    }

    public void Dispose()
    {
        //DeleteWorkflow(Workflow.Name);
    }

    /// <summary>
    /// Create a random ID adding the prefix "workflow-cs-test-".
    /// </summary>
    /// <param name="workflowName">The workflow's ID</param>
    ///
    public string GetRandomWorkflowId()
    {
        return $"workflow-cs-test-{Guid.NewGuid()}";
    }
}
