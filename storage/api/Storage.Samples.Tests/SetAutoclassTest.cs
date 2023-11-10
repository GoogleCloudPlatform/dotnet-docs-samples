// Copyright 2022 Google Inc.
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

using System;
using Xunit;
using static Google.Apis.Storage.v1.Data.Bucket;

[Collection(nameof(StorageFixture))]
public class SetAutoclassTest
{
    private readonly StorageFixture _fixture;
    public SetAutoclassTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void SetBucketAutoclassConfiguration()
    {
        var bucketName = Guid.NewGuid().ToString();
        _fixture.CreateBucket(bucketName, autoclassData: new AutoclassData { Enabled = true });
        var setAutoclassSample = new SetAutoclassSample();
        var bucket = setAutoclassSample.SetAutoclass(bucketName);
        Assert.Equal(true, bucket.Autoclass.Enabled);
        Assert.Equal("ARCHIVE", bucket.Autoclass.TerminalStorageClass);
    }
}
