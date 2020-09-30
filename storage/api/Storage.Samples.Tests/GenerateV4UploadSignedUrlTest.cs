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

using System;
using Xunit;

[Collection(nameof(BucketFixture))]
public class GenerateV4UploadSignedUrlTest
{
    private readonly BucketFixture _bucketFixture;

    public GenerateV4UploadSignedUrlTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestGenerateV4UploadSignedUrl()
    {
        GenerateV4UploadSignedUrlSample generateV4UploadSignedUrlSample = new GenerateV4UploadSignedUrlSample();
        var credentialFilePath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        var signedUrl = generateV4UploadSignedUrlSample.GenerateV4UploadSignedUrl(_bucketFixture.BucketNameGeneric, _bucketFixture.FileName, credentialFilePath);
        Assert.NotNull(signedUrl);
    }
}
