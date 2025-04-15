// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

using Google;
using System;
using System.Net;
using Xunit;

[Collection(nameof(StorageFixture))]
public class MoveObjectWithMetaGenerationMismatchFailsTest
{
    private readonly StorageFixture _fixture;

    public MoveObjectWithMetaGenerationMismatchFailsTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MoveObjectWithMetaGenerationMismatchFails()
    {
        MoveObjectMetaGenerationMismatchFailsSample moveObjectMetaGenerationMismatchFailsSample = new MoveObjectMetaGenerationMismatchFailsSample();
        UploadObjectFromMemorySample uploadObjectFromMemory = new UploadObjectFromMemorySample();
        var originName = Guid.NewGuid().ToString();
        var originContent = Guid.NewGuid().ToString();
        var destinationName = Guid.NewGuid().ToString();
        uploadObjectFromMemory.UploadObjectFromMemory(_fixture.BucketNameHns, originName, originContent);
        var exception = Assert.Throws<GoogleApiException>(() => moveObjectMetaGenerationMismatchFailsSample.MoveObjectMetaGenerationMismatchFails(_fixture.BucketNameHns, originName, destinationName, 0));
        _fixture.CollectHnsObject(originName);
        Assert.Equal(HttpStatusCode.PreconditionFailed, exception.HttpStatusCode);
    }
}
