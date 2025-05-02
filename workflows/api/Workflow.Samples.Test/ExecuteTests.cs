/*
 * Copyright 2025 Google LLC
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


using Google.Cloud.Workflows.Executions.V1;

[Collection(nameof(WorkflowFixture))]
public class ExecuteTests
{
    private readonly ExecuteWorkflowSample _sample;
    private readonly WorkflowFixture _fixture;

    public ExecuteTests(WorkflowFixture fixture)
    {
        _fixture = fixture;
        _sample = new ExecuteWorkflowSample();
    }

    [Fact]
    public async Task Execute()
    {
        Task<Execution> executionTask = _sample.ExecuteWorkflow(_fixture.ProjectId, _fixture.LocationId, _fixture.WorkflowID);
        var completedTask = await Task.WhenAny(executionTask, Task.Delay(TimeSpan.FromMinutes(10)));
        if (completedTask != executionTask)
        {
           throw new TimeoutException("The operation has timed out.");
        }
        var execution = await executionTask;
        // When creating an execution a name is assigned, so check if it is not null.
        Assert.NotNull(execution.Name);
    }
}
