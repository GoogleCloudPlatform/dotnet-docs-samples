// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Xunit;

[Collection(nameof(StorageFixture))]
public class CreateBucketWithHierarchicalNamespaceEnabledTest
{
    private readonly StorageFixture _fixture;

    public CreateBucketWithHierarchicalNamespaceEnabledTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CreateBucketWithHierarchicalNamespaceEnabled()
    {
        CreateBucketWithHierarchicalNamespaceEnabledSample sample =
            new CreateBucketWithHierarchicalNamespaceEnabledSample();

        var bucketName = Guid.NewGuid().ToString();
        var bucket = sample.CreateBucketWithHierarchicalNamespace(_fixture.ProjectId, bucketName);
        _fixture.SleepAfterBucketCreateUpdateDelete();
        _fixture.TempBucketNames.Add(bucket.Name);

        Assert.True(bucket.HierarchicalNamespace.Enabled);
    }
}
