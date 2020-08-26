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

using System.IO;
using Xunit;

[Collection(nameof(BucketFixture))]
public class DownloadFileRequesterPaysTest
{
    private readonly BucketFixture _bucketFixture;

    public DownloadFileRequesterPaysTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestDownloadFileRequesterPays()
    {
        var downloadFileRequesterPaysSample = new DownloadFileRequesterPaysSample();

        // download file request pays
        downloadFileRequesterPaysSample.DownloadFileRequesterPays(_bucketFixture.ProjectId, _bucketFixture.BucketNameGeneric, _bucketFixture.FileName, "HelloDownloadObjectRequesterPays2.txt");
        Assert.Equal(File.ReadAllText(_bucketFixture.FilePath), File.ReadAllText("HelloDownloadObjectRequesterPays2.txt"));
        File.Delete("HelloDownloadObjectRequesterPays2.txt");
    }
}
