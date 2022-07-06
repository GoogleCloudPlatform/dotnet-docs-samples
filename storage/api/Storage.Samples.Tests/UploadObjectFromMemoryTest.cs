
// Copyright 2021 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Xunit;
using System.Linq;
using System;

[Collection(nameof(StorageFixture))]
public class UploadObjectFromMemoryTest
{
    private readonly StorageFixture _fixture;

    public UploadObjectFromMemoryTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void UploadObjectFromMemory()
    {
        UploadObjectFromMemorySample uploadObjectFromMemorySample = new UploadObjectFromMemorySample();
        ListFilesSample listFilesSample = new ListFilesSample();

        var fileName = Guid.NewGuid().ToString();
        uploadObjectFromMemorySample.UploadObjectFromMemory(_fixture.BucketNameGeneric, fileName, "This is testing content");
        _fixture.Collect(fileName);
        var files = listFilesSample.ListFiles(_fixture.BucketNameGeneric);
        Assert.Contains(files, c => c.Name == fileName && c.Size > 0);
    }
}
