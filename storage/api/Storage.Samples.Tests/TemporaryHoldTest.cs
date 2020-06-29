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
public class TemporaryHoldTest
{
    private readonly BucketFixture _bucketFixture;

    public TemporaryHoldTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TemporaryHold()
    {
        SetTemporarydHoldSample setTemporarydHoldSample = new SetTemporarydHoldSample();
        ReleaseTemporaryHoldSample releaseTemporaryHoldSample = new ReleaseTemporaryHoldSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        uploadFileSample.UploadFile(_bucketFixture.BucketName1, _bucketFixture.FilePath, _bucketFixture.CollectRegionalObject("TemporaryHold.txt"));

        setTemporarydHoldSample.SetTemporarydHold(_bucketFixture.BucketName1, "TemporaryHold.txt");

        var metadata = getMetadataSample.GetMetadata(_bucketFixture.BucketName1, "TemporaryHold.txt");
        Assert.True(metadata.TemporaryHold);

        releaseTemporaryHoldSample.ReleaseTemporaryHold(_bucketFixture.BucketName1, "TemporaryHold.txt");

        metadata = getMetadataSample.GetMetadata(_bucketFixture.BucketName1, "TemporaryHold.txt");
        Assert.False(metadata.TemporaryHold);
    }
}
