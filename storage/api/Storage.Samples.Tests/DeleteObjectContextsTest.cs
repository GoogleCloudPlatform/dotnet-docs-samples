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
public class DeleteObjectContextsTest
{
    private readonly StorageFixture _fixture;

    public DeleteObjectContextsTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void DeleteObjectContexts()
    {
        DeleteObjectContextsSample deleteContextsSample = new DeleteObjectContextsSample();
        SetObjectContextsSample setContextsSample = new SetObjectContextsSample();
        UploadObjectFromMemorySample uploadObjectSample = new UploadObjectFromMemorySample();

        var customContexts = new Dictionary<string, ObjectCustomContextPayload>
        {
            { "context-key-1", new ObjectCustomContextPayload { Value = "context-value-1" } },
            { "context-key-2", new ObjectCustomContextPayload { Value = "context-value-2" } }
        };

        var bucketName = _fixture.GenerateBucketName();
        var objectName = _fixture.GenerateName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        var content = _fixture.GenerateContent();
        uploadObjectSample.UploadObjectFromMemory(bucketName, objectName, content);

        // Given: the object has custom contexts.
        var appliedContexts = setContextsSample.SetObjectContexts(bucketName, objectName, customContexts);
        Assert.Equal(2, appliedContexts.Custom.Count);

        // When: all contexts are deleted.
        var updatedObject = deleteContextsSample.DeleteObjectContexts(bucketName, objectName);

        // Then: the returned object and a fresh fetch both have no custom contexts.
        Assert.True(updatedObject.Contexts?.Custom == null || updatedObject.Contexts.Custom.Count == 0);
        var retrievedObject = _fixture.Client.GetObject(bucketName, objectName);
        Assert.True(retrievedObject.Contexts?.Custom == null || retrievedObject.Contexts.Custom.Count == 0);
    }
}
