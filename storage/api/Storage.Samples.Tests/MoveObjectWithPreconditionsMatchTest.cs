// Copyright 2025 Google LLC
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
public class MoveObjectWithPreconditionsMatchTest
{
    private readonly StorageFixture _fixture;

    public MoveObjectWithPreconditionsMatchTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MoveObjectWithPreconditionsMatch()
    {
        MoveObjectPreconditionsMatchSample moveObjectPreconditionsMatch = new MoveObjectPreconditionsMatchSample();
        UploadObjectFromMemorySample uploadObjectFromMemory = new UploadObjectFromMemorySample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        ListFilesSample listFilesSample = new ListFilesSample();
        var originName = Guid.NewGuid().ToString();
        var originContent = Guid.NewGuid().ToString();
        var destinationName = Guid.NewGuid().ToString();
        uploadObjectFromMemory.UploadObjectFromMemory(_fixture.BucketNameHns, originName, originContent);
        var originMetaData = getMetadataSample.GetMetadata(_fixture.BucketNameHns, originName);
        moveObjectPreconditionsMatch.MoveObjectPreconditionsMatch(_fixture.BucketNameHns, originName, destinationName, originMetaData.Generation, originMetaData.Metageneration);
        _fixture.CollectHnsObject(destinationName);
        var objects = listFilesSample.ListFiles(_fixture.BucketNameHns);
        Assert.DoesNotContain(objects, obj => obj.Name == originName);
        Assert.Contains(objects, obj => obj.Name == destinationName);
    }
}
