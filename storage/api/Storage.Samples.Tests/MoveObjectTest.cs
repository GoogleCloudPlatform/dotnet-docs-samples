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
public class MoveObjectTest
{
    private readonly StorageFixture _fixture;

    public MoveObjectTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MoveObject()
    {
        MoveObjectSample moveObjectSample = new MoveObjectSample();
        UploadObjectFromMemorySample uploadObjectFromMemory = new UploadObjectFromMemorySample();
        ListFilesSample listFilesSample = new ListFilesSample();
        var originName = Guid.NewGuid().ToString();
        var originContent = Guid.NewGuid().ToString();
        var destinationName = Guid.NewGuid().ToString();
        uploadObjectFromMemory.UploadObjectFromMemory(_fixture.BucketNameGeneric, originName, originContent);
        moveObjectSample.MoveObject(_fixture.BucketNameGeneric, originName, destinationName);
        _fixture.Collect(destinationName);
        var objects = listFilesSample.ListFiles(_fixture.BucketNameGeneric);
        Assert.DoesNotContain(objects, obj => obj.Name == originName);
        Assert.Contains(objects, obj => obj.Name == destinationName);
    }
}
