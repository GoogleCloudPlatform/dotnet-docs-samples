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

using Xunit;

[Collection(nameof(StorageFixture))]
public class MoveObjectTest
{
    private readonly StorageFixture _fixture;

    public MoveObjectTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    // Moves the source object to destination object within a hierarchical namespace enabled bucket.
    [Fact]
    public void MoveObject()
    {
        MoveObjectSample moveObjectSample = new MoveObjectSample();
        ListFilesSample listFilesSample = new ListFilesSample();
        moveObjectSample.MoveObject(_fixture.BucketNameHns, _fixture.FileName, "CopyFile.txt");
        var files = listFilesSample.ListFiles(_fixture.BucketNameHns);
        Assert.Contains(files, c => c.Name == "CopyFile.txt");
    }
}
