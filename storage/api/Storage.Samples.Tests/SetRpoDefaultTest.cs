﻿// Copyright 2021 Google Inc.
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

[Collection(nameof(StorageFixture))]
public class SetRpoDefaultTest
{
    private readonly StorageFixture _fixture;

    public SetRpoDefaultTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void SetRpoDefault()
    {
        SetRpoDefaultSample setRPODefaultSample = new SetRpoDefaultSample();
        GetRpoSample getRpoSample = new GetRpoSample();

        var bucketName = Guid.NewGuid().ToString();
        _fixture.CreateBucket(bucketName, "nam4");

        setRPODefaultSample.SetRpoDefault(bucketName);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        var rpo = getRpoSample.GetRpo(bucketName);
        Assert.Equal("DEFAULT", rpo);
    }
}

