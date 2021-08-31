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

[Collection(nameof(StorageFixture))]
public class ReleaseTemporaryHoldTest
{
    private readonly StorageFixture _fixture;

    public ReleaseTemporaryHoldTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestReleaseTemporaryHold()
    {
        SetTemporaryHoldSample setTemporaryHoldSample = new SetTemporaryHoldSample();
        ReleaseTemporaryHoldSample releaseTemporaryHoldSample = new ReleaseTemporaryHoldSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        uploadFileSample.UploadFile(_fixture.BucketNameRegional, _fixture.FilePath, _fixture.CollectRegionalObject("ReleaseTemporaryHold.txt"));

        setTemporaryHoldSample.SetTemporaryHold(_fixture.BucketNameRegional, "ReleaseTemporaryHold.txt");

        releaseTemporaryHoldSample.ReleaseTemporaryHold(_fixture.BucketNameRegional, "ReleaseTemporaryHold.txt");

        var metadata = getMetadataSample.GetMetadata(_fixture.BucketNameRegional, "ReleaseTemporaryHold.txt");
        Assert.False(metadata.TemporaryHold);
    }
}
