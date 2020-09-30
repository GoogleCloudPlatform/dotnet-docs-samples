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
using Xunit;

[Collection(nameof(BucketFixture))]
public class UploadFileRequesterPaysTest
{
    private readonly BucketFixture _bucketFixture;

    public UploadFileRequesterPaysTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestUploadFileRequesterPays()
    {
        var downloadFileRequesterPaysSample = new DownloadFileRequesterPaysSample();
        UploadFileRequesterPaysSample uploadFileRequesterPaysSample = new UploadFileRequesterPaysSample();

        // Upload file request pays.
        uploadFileRequesterPaysSample.UploadFileRequesterPays(_bucketFixture.ProjectId, _bucketFixture.BucketNameGeneric, _bucketFixture.FilePath,
            _bucketFixture.Collect("HelloUploadObjectRequesterPays.txt"));

        // Download file request pays.
        downloadFileRequesterPaysSample.DownloadFileRequesterPays(_bucketFixture.ProjectId, _bucketFixture.BucketNameGeneric, "HelloUploadObjectRequesterPays.txt", "HelloUploadObjectRequesterPays2.txt");
        Assert.Equal(File.ReadAllText(_bucketFixture.FilePath), File.ReadAllText("HelloUploadObjectRequesterPays2.txt"));
        File.Delete("HelloUploadObjectRequesterPays2.txt");
    }
}
