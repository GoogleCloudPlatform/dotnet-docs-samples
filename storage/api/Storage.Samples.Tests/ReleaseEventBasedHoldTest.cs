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
public class ReleaseEventBasedHoldTest
{
    private readonly StorageFixture _fixture;

    public ReleaseEventBasedHoldTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestReleaseEventBasedHold()
    {
        ReleaseEventBasedHoldSample releaseEventBasedHoldSample = new ReleaseEventBasedHoldSample();
        SetEventBasedHoldSample setEventBasedHoldSample = new SetEventBasedHoldSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, _fixture.FilePath, _fixture.Collect("ReleaseEventBasedHold.txt"));

        // Set event based hold.
        setEventBasedHoldSample.SetEventBasedHold(_fixture.BucketNameGeneric, "ReleaseEventBasedHold.txt");

        // Release event based hold.
        releaseEventBasedHoldSample.ReleaseEventBasedHold(_fixture.BucketNameGeneric, "ReleaseEventBasedHold.txt");
        var metadata = getMetadataSample.GetMetadata(_fixture.BucketNameGeneric, "ReleaseEventBasedHold.txt");
        Assert.False(metadata.EventBasedHold);
    }
}
