/*
 * Copyright 2022 Google LLC
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

using Google.Cloud.Video.LiveStream.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

[CollectionDefinition(nameof(LiveStreamFixture))]
public class LiveStreamFixture : IDisposable, IAsyncLifetime, ICollectionFixture<LiveStreamFixture>
{
    public string ProjectId { get; }
    public string LocationId { get; } = "us-central1";
    public string InputIdPrefix { get; } = "test-input";

    public System.Int32 UpdateTopPixels { get; } = 5;
    public System.Int32 UpdateBottomPixels { get; } = 5;

    public List<string> InputIds { get; } = new List<string>();

    public Input TestInput { get; set; }
    public string TestInputId { get; set; }

    private readonly CreateInputSample _createInputSample = new CreateInputSample();
    private readonly DeleteInputSample _deleteInputSample = new DeleteInputSample();

    public LiveStreamFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }
    }

    public async Task InitializeAsync()
    {
        TestInputId = $"{InputIdPrefix}-{RandomId()}";
        InputIds.Add(TestInputId);
        TestInput = await _createInputSample.CreateInputAsync(ProjectId, LocationId, TestInputId);
    }

    public async Task DisposeAsync()
    {
        foreach (string id in InputIds)
        {
            try
            {
                _deleteInputSample.DeleteInputAsync(ProjectId, LocationId, id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed for input: " + id + " with error: " + e.ToString());
            }
        }
    }

    public void Dispose()
    {
    }

    public string RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }
}