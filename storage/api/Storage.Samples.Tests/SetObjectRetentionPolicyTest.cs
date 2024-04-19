// Copyright 2024 Google LLC
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

using System;
using Xunit;

[Collection(nameof(StorageFixture))]

public class SetObjectRetentionPolicyTest
{
    private readonly StorageFixture _fixture;

    public SetObjectRetentionPolicyTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]

    public void SetObjectRetentionPolicy()
    {
        CreateBucketWithObjectRetentionSample createBucketWithObjectRetention =
            new CreateBucketWithObjectRetentionSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        SetObjectRetentionPolicySample setObjectRetentionPolicySample = new SetObjectRetentionPolicySample();

        var objectName = "HelloSetObjectMetadata.txt";

        var bucketName = Guid.NewGuid().ToString();
        var bucket = createBucketWithObjectRetention.CreateBucketWithObjectRetention(_fixture.ProjectId, bucketName);
        _fixture.SleepAfterBucketCreateUpdateDelete();
        _fixture.TempBucketNames.Add(bucket.Name);

        uploadFileSample.UploadFile(bucketName, _fixture.FilePath, _fixture.Collect(objectName));

        var file = setObjectRetentionPolicySample.SetObjectRetentionPolicy(bucketName, objectName);

        Assert.NotNull(file.Retention);
    }

}
