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

using Google.Cloud.StorageBatchOperations.V1;
using System;
using System.IO;
using System.Text;
using Xunit;

[Collection(nameof(StorageFixture))]
public class CreateBatchJobTest
{
    private readonly StorageFixture _fixture;
    private readonly BucketList.Types.Bucket _bucket = new();
    private readonly BucketList _bucketList = new();
    private readonly PrefixList _prefixListObject = new();

    public CreateBatchJobTest(StorageFixture fixture)
    {
        _fixture = fixture;
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: false, registerForDeletion: true);
        var manifestBucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(manifestBucketName, multiVersion: false, softDelete: false, registerForDeletion: true);
        var objectName = _fixture.GenerateName();
        var manifestObjectName = _fixture.GenerateName();
        var objectContent = _fixture.GenerateContent();
        var manifestObjectContent = $"bucket,name,generation{Environment.NewLine}{bucketName},{objectName}";
        byte[] byteObjectContent = Encoding.UTF8.GetBytes(objectContent);
        MemoryStream streamObjectContent = new MemoryStream(byteObjectContent);
        _fixture.Client.UploadObject(bucketName, objectName, "application/text", streamObjectContent);
        byte[] byteManifestObjectContent = Encoding.UTF8.GetBytes(manifestObjectContent);
        MemoryStream streamManifestObjectContent = new MemoryStream(byteManifestObjectContent);
        _fixture.Client.UploadObject(manifestBucketName, $"{manifestObjectName}.csv", "text/csv", streamManifestObjectContent);

        _bucket = new BucketList.Types.Bucket
        {
            Bucket_ = bucketName,
            PrefixList = _prefixListObject,
            Manifest = new Manifest { ManifestLocation = $"gs://{manifestBucketName}/{manifestObjectName}.csv" }
        };
        _bucketList.Buckets.Insert(0, _bucket);
    }

    [Fact]
    public void CreateBatchJob()
    {
        CreateBatchJobSample createJob = new CreateBatchJobSample();
        var jobId = _fixture.GenerateJobId();
        var jobTransformationCase = "DeleteObject";
        var holdState = "EventBasedHoldSet";
        string jobType;
        if (jobTransformationCase == "PutObjectHold")
        {
            jobType = $"{jobTransformationCase}{holdState}";
        }
        else
        {
            jobType = jobTransformationCase;
        }

        var createdBatchJob = createJob.CreateBatchJob(_fixture.LocationName, _bucketList, jobId, jobType);
        Assert.Equal(createdBatchJob.BucketList, _bucketList);
        Assert.Equal(createdBatchJob.TransformationCase.ToString(), jobTransformationCase);
        Assert.Equal(createdBatchJob.SourceCase.ToString(), _bucketList.GetType().Name);
        Assert.NotNull(createdBatchJob.Name);
        Assert.NotNull(createdBatchJob.JobName);
        Assert.NotNull(createdBatchJob.CreateTime);
        Assert.NotNull(createdBatchJob.CompleteTime);
    }
}
