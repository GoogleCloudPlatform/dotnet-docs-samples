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

using Google.LongRunning;
using Google.Cloud.Workflows.Common.V1;
using Google.Cloud.Workflows.V1;
using Google.Api.Gax.ResourceNames;

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
        WorkflowID = RandomId();

        // Create workflow with the given ID.
        Workflow = CreateWorkflow(WorkflowID);

    }

    /// <summary>
    /// Create a workflow by the given id and return it.
    ///</summary>
    /// <param name="workflowID">The workflow's ID</param>
    /// 
    public Workflow CreateWorkflow(string workflowID)
    {
        WorkflowsClient client = WorkflowsClient.Create();

        string parent = LocationName.Format(ProjectId, LocationId);
        string filePath = Path.Combine(AppContext.BaseDirectory, "myFirstWorkflow.yaml");
        string content = File.ReadAllText(filePath);

        Workflow workflow = new Workflow
        {
            Name = WorkflowName.Format(ProjectId, LocationId, workflowID),
            SourceContents = content
        };

        CreateWorkflowRequest req = new CreateWorkflowRequest
        {
            Parent = parent,
            Workflow = workflow,
            WorkflowId = workflowID
        };


        Operation<Workflow, OperationMetadata> operation = client.CreateWorkflow(req);
        Operation<Workflow, OperationMetadata> deployedWorkflow = operation.PollUntilCompletedAsync().GetAwaiter().GetResult();

        workflow = deployedWorkflow.Result;

        return workflow;
    }

     /// <summary>
    /// Delete a workflow by the given workflow name and return it.
    ///</summary>
    /// <param name="workflowName">The workflow's ID</param>
    /// 
    public void DeleteWorkflow(string workflowName)
    {
        WorkflowsClient client = WorkflowsClient.Create();

        DeleteWorkflowRequest req = new DeleteWorkflowRequest
        {
            Name = workflowName,
        };

        try
        {
            client.DeleteWorkflow(req);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - secret was already deleted
        }
    }

    public void Dispose()
    {
        DeleteWorkflow(Workflow.Name);
    }

    public string RandomId()
    {
        return $"workflow-cs-test-{Guid.NewGuid()}";
    }
}