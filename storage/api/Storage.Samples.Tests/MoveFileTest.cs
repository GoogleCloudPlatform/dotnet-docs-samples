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

using Google;
using Xunit;

[Collection(nameof(StorageFixture))]
public class MoveFileTest
{
    private readonly StorageFixture _fixture;

    public MoveFileTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MoveFile()
    {
        MoveFileSample moveFileSample = new MoveFileSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();

        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, _fixture.FilePath, "HelloMove.txt");
        // Make sure the file doesn't exist until we move it there.
        Assert.Throws<GoogleApiException>(() => getMetadataSample.GetMetadata(_fixture.BucketNameGeneric, "ByeMove.txt"));

        moveFileSample.MoveFile(_fixture.BucketNameGeneric, "HelloMove.txt", _fixture.BucketNameGeneric, _fixture.Collect("ByeMove.txt"));

        var exception = Assert.Throws<GoogleApiException>(() => getMetadataSample.GetMetadata(_fixture.BucketNameGeneric, "HelloMove.txt"));
        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.HttpStatusCode);
    }
}
