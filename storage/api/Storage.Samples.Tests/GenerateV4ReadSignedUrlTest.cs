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

using System;
using System.Net.Http;
using Xunit;

[Collection(nameof(StorageFixture))]
public class GenerateV4ReadSignedUrlTest
{
    private readonly StorageFixture _fixture;

    public GenerateV4ReadSignedUrlTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async void GenerateV4ReadSignedUrl()
    {
        GenerateV4SignedReadUrlSample generateV4SignedReadUrlSample = new GenerateV4SignedReadUrlSample();
        var credentialFilePath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        var signedUrl = generateV4SignedReadUrlSample.GenerateV4SignedReadUrl(_fixture.BucketNameGeneric, _fixture.FileName, credentialFilePath);

        using var client = new HttpClient();
        var response = await client.GetAsync(signedUrl);
        Assert.InRange((int)response.StatusCode, 200, 299);
    }
}
