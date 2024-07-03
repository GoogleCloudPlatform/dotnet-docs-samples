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
public class StorageControlCreateManagedFolderTest
{
    private readonly StorageFixture _fixture;

    public StorageControlCreateManagedFolderTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestStorageControlCreateManagedFolder()
    {
        StorageControlCreateManagedFolderSample sample = new StorageControlCreateManagedFolderSample();
        var managedFolder = sample.StorageControlCreateManagedFolder(_fixture.BucketNameHns, "createTestManagedFolder");
        Assert.Contains("createTestManagedFolder", managedFolder.Name);
    }
}
