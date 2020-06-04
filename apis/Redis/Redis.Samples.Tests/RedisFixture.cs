// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Xunit;

[CollectionDefinition(nameof(RedisFixture))]
public class RedisFixture : IDisposable, ICollectionFixture<RedisFixture>
{
    private readonly List<string> _instanceIdsToDelete = new List<string>();
    public string ProjectId { get; }
    public string LocationId { get; }
    public string InstanceId { get; private set; } = $"csharp-{Guid.NewGuid().ToString().Substring(0, 20)}";

    public RedisFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        if (string.IsNullOrWhiteSpace(ProjectId))
        {
            throw new Exception("Please set the GOOGLE_PROJECT_ID Environment Variable to your project ID.");
        }

        LocationId = "us-east1";
        CreateInstanceSample createInstanceSample = new CreateInstanceSample();
        createInstanceSample.CreateInstance(ProjectId, LocationId, InstanceId);
        MarkForDeletion(InstanceId);
    }

    public void Dispose()
    {
        DeleteInstanceSample deleteInstanceSample = new DeleteInstanceSample();
        _instanceIdsToDelete.ForEach(instanceId =>
        {
            deleteInstanceSample.DeleteInstance(ProjectId, LocationId, instanceId);
        });
    }

    public void MarkForDeletion(string instanceId)
    {
        _instanceIdsToDelete.Add(instanceId);
    }
}
