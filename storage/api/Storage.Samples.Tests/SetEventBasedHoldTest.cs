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
public class SetEventBasedHoldTest
{
    private readonly BucketFixture _bucketFixture;

    public SetEventBasedHoldTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestSetEventBasedHold()
    {
        ReleaseEventBasedHoldSample releaseEventBasedHoldSample = new ReleaseEventBasedHoldSample();
        SetEventBasedHoldSample setEventBasedHoldSample = new SetEventBasedHoldSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, _bucketFixture.Collect("SetEventBasedHold.txt"));

        // Set event based hold.
        setEventBasedHoldSample.SetEventBasedHold(_bucketFixture.BucketNameGeneric, "SetEventBasedHold.txt");
        var metadata = getMetadataSample.GetMetadata(_bucketFixture.BucketNameGeneric, "SetEventBasedHold.txt");
        Assert.True(metadata.EventBasedHold);

        // Release event based hold.
        releaseEventBasedHoldSample.ReleaseEventBasedHold(_bucketFixture.BucketNameGeneric, "SetEventBasedHold.txt");
    }
}
