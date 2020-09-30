// Copyright 2020 Google Inc.
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

[Collection(nameof(BucketFixture))]
public class ReleaseEventBasedHoldTest
{
    private readonly BucketFixture _bucketFixture;

    public ReleaseEventBasedHoldTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestReleaseEventBasedHold()
    {
        ReleaseEventBasedHoldSample releaseEventBasedHoldSample = new ReleaseEventBasedHoldSample();
        SetEventBasedHoldSample setEventBasedHoldSample = new SetEventBasedHoldSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, _bucketFixture.Collect("ReleaseEventBasedHold.txt"));

        // Set event based hold.
        setEventBasedHoldSample.SetEventBasedHold(_bucketFixture.BucketNameGeneric, "ReleaseEventBasedHold.txt");

        // Release event based hold.
        releaseEventBasedHoldSample.ReleaseEventBasedHold(_bucketFixture.BucketNameGeneric, "ReleaseEventBasedHold.txt");
        var metadata = getMetadataSample.GetMetadata(_bucketFixture.BucketNameGeneric, "ReleaseEventBasedHold.txt");
        Assert.False(metadata.EventBasedHold);
    }
}
