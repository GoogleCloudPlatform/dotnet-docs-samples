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

using System;
using Xunit;

[Collection(nameof(StorageFixture))]
public class SetRpoAsyncTurboTest
{
    private readonly StorageFixture _fixture;

    public SetRpoAsyncTurboTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void SetRpoAsyncTurbo()
    {
        SetRpoAsyncTurboSample setRpoAsyncTurboSample = new SetRpoAsyncTurboSample();
        GetRpoSample getRpoSample = new GetRpoSample();

        // Enabling turbo replication requires a bucket with dual-region configuration
        var bucketName = Guid.NewGuid().ToString();
        _fixture.CreateBucket(bucketName,"nam4");

        setRpoAsyncTurboSample.SetRpoAsyncTurbo(bucketName);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        var rpo = getRpoSample.GetRpo(bucketName);
        Assert.Equal("ASYNC_TURBO", rpo);
    }
}
