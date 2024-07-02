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

using Xunit;

namespace Storage.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class StorageControlGetFolderTest
{
    private readonly StorageFixture _fixture;

    public StorageControlGetFolderTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestStorageControlGetFolder()
    {
        StorageControlCreateFolderSample createFolderSample = new StorageControlCreateFolderSample();
        createFolderSample.StorageControlCreateFolder(_fixture.BucketNameHns, "getTestFolder");

        StorageControlGetFolderSample getFolderSample = new StorageControlGetFolderSample();
        var folder = getFolderSample.StorageControlGetFolder(_fixture.BucketNameHns, "getTestFolder");

        Assert.Contains("getTestFolder", folder.Name);
    }
}
