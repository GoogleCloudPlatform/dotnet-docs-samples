﻿// Copyright 2020 Google Inc.
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

[Collection(nameof(StorageFixture))]
public class SetTemporaryHoldTest
{
    private readonly StorageFixture _fixture;

    public SetTemporaryHoldTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestSetTemporaryHold()
    {
        SetTemporaryHoldSample setTemporaryHoldSample = new SetTemporaryHoldSample();
        ReleaseTemporaryHoldSample releaseTemporaryHoldSample = new ReleaseTemporaryHoldSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        uploadFileSample.UploadFile(_fixture.BucketNameRegional, _fixture.FilePath, _fixture.CollectRegionalObject("SetTemporaryHold.txt"));

        setTemporaryHoldSample.SetTemporaryHold(_fixture.BucketNameRegional, "SetTemporaryHold.txt");

        var metadata = getMetadataSample.GetMetadata(_fixture.BucketNameRegional, "SetTemporaryHold.txt");
        Assert.True(metadata.TemporaryHold);

        releaseTemporaryHoldSample.ReleaseTemporaryHold(_fixture.BucketNameRegional, "SetTemporaryHold.txt");
    }
}
