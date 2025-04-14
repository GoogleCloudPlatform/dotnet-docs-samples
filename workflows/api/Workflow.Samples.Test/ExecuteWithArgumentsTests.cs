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
public class ExecuteWithArgumentsTests
{
    private readonly ExecuteWorkfloWithArgumentswSample _sample;
    private readonly WorkflowFixture _fixture;

    public ExecuteWithArgumentsTests(WorkflowFixture fixture)
    {
        _fixture = fixture;
        _sample = new ExecuteWorkfloWithArgumentswSample();
    }

    [Fact]
    public async Task ExecuteWithArguments()
    {
        var cts = new CancellationTokenSource();
        Execution execution = new Execution();

        using (var timeoutCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cts.Token))
        {
            Task<Execution> task = _sample.ExecuteWorkflowWithArguments(_fixture.ProjectId, _fixture.LocationId, _fixture.WorkflowID);

            var completedTask = await Task.WhenAny(task, Task.Delay(TimeSpan.FromMinutes(10), timeoutCancellationTokenSource.Token));
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                execution = await task;
            }
            else
            {
                throw new TimeoutException("The operation has timed out.");
            }
        }
        ;

        Assert.Equal(Execution.Types.State.Succeeded, execution.State);
        Assert.Contains("Cloud", execution.Argument);
    }
}
