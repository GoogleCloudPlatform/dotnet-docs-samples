// Copyright 2026 Google LLC
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

using Google.Apis.Storage.v1.Data;
using System.Collections.Generic;
using Xunit;

[Collection(nameof(StorageFixture))]
public class GetObjectContextsTest
{
    private readonly StorageFixture _fixture;

    public GetObjectContextsTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void GetObjectContexts()
    {
        GetObjectContextsSample getContextsSample = new GetObjectContextsSample();
        SetObjectContextsSample setContextsSample = new SetObjectContextsSample();
        UploadObjectFromMemorySample uploadObjectSample = new UploadObjectFromMemorySample();

        string contextKey = "A\u00F1\u03A9\U0001F680";
        string contextValue = "Ab\u00F1\u03A9\U0001F680";

        var customContexts = new Dictionary<string, ObjectCustomContextPayload>
        {
            { contextKey, new ObjectCustomContextPayload { Value = contextValue } }
        };

        var bucketName = _fixture.GenerateBucketName();
        var objectName = _fixture.GenerateName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        var content = _fixture.GenerateContent();
        uploadObjectSample.UploadObjectFromMemory(bucketName, objectName, content);
        var appliedContexts = setContextsSample.SetObjectContexts(bucketName, objectName, customContexts);
        var retrievedContexts = getContextsSample.GetObjectContexts(bucketName, objectName);
        Assert.Equal(appliedContexts.Custom.Count, retrievedContexts.Custom.Count);
        var singleContext = Assert.Single(retrievedContexts.Custom);
        Assert.Equal(contextKey, singleContext.Key);
        Assert.Equal(contextValue, singleContext.Value.Value);
    }
}
