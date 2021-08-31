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

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(StorageFixture))]
public class DownloadByteRangeAsyncTest
{
    private readonly StorageFixture _fixture;

    public DownloadByteRangeAsyncTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DownloadByteRangeAsync()
    {
        UploadFileSample uploadFileSample = new UploadFileSample();
        DownloadByteRangeAsyncSample downloadByteRangeSample = new DownloadByteRangeAsyncSample();
        uploadFileSample.UploadFile(
            _fixture.BucketNameGeneric, "Resources/HelloDownloadCompleteByteRange.txt",
            _fixture.Collect("HelloDownloadCompleteByteRange.txt"));

        await downloadByteRangeSample.DownloadByteRangeAsync(
            _fixture.BucketNameGeneric, "HelloDownloadCompleteByteRange.txt", 0, 20,
            "HelloDownloadCompleteByteRange.txt_0-20");

        var downloadedString = Encoding.UTF8.GetString(File.ReadAllBytes("HelloDownloadCompleteByteRange.txt_0-20"));
        Assert.Equal("\uFEFFHello Download Com", downloadedString);
        File.Delete("HelloDownloadCompleteByteRange.txt_0-20");
    }
}
