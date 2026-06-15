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
public class CreateBatchJobWithPutMetaDataTest
{
    private readonly StorageFixture _fixture;
    private readonly BucketList.Types.Bucket _bucket = new();
    private readonly BucketList _bucketList = new();
    private readonly PrefixList _prefixListObject = new();

    public CreateBatchJobWithPutMetaDataTest(StorageFixture fixture)
    {
        _fixture = fixture;
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: false, registerForDeletion: true);

        var manifestBucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(manifestBucketName, multiVersion: false, softDelete: false, registerForDeletion: true);

        var objectName = _fixture.GenerateGuid();
        var manifestObjectName = _fixture.GenerateGuid();
        var objectContent = _fixture.GenerateGuid();
        var manifestObjectContent = $"bucket,name,generation{Environment.NewLine}{bucketName},{objectName}";

        byte[] byteObjectContent = Encoding.UTF8.GetBytes(objectContent);
        using MemoryStream streamObjectContent = new MemoryStream(byteObjectContent);
        // Uploading an object to the bucket
        _fixture.Client.UploadObject(bucketName, objectName, "application/text", streamObjectContent);

        byte[] byteManifestObjectContent = Encoding.UTF8.GetBytes(manifestObjectContent);
        // Uploading a manifest object to the manifest bucket
        using MemoryStream streamManifestObjectContent = new MemoryStream(byteManifestObjectContent);
        _fixture.Client.UploadObject(manifestBucketName, $"{manifestObjectName}.csv", "text/csv", streamManifestObjectContent);
        _bucket = new BucketList.Types.Bucket
        {
            Bucket_ = bucketName,
            // The prefix list is used to specify the objects to be transformed. To match all objects, use an empty list.
            PrefixList = _prefixListObject,
            // Manifest location contains csv file having list of objects to be transformed"
            Manifest = new Manifest { ManifestLocation = $"gs://{manifestBucketName}/{manifestObjectName}.csv" }
        };
        // Adding the bucket to the bucket list.
        _bucketList.Buckets.Insert(0, _bucket);
    }

    [Fact]
    public void TestCreateBatchJobWithPutMetaData()
    {
        CreateBatchJobWithPutMetadataSample createJob = new CreateBatchJobWithPutMetadataSample();
        var jobId = _fixture.GenerateGuid();

        var createdBatchJob = createJob.CreateBatchJobWithPutMetadata(_fixture.LocationName, _bucketList, jobId);
        Assert.Equal(createdBatchJob.BucketList, _bucketList);
        Assert.Equal("PutMetadata", createdBatchJob.TransformationCase.ToString());
        Assert.Equal(createdBatchJob.SourceCase.ToString(), _bucketList.GetType().Name);
        Assert.NotNull(createdBatchJob.Name);
        Assert.NotNull(createdBatchJob.JobName);
        Assert.NotNull(createdBatchJob.CreateTime);
        Assert.NotNull(createdBatchJob.CompleteTime);
        _fixture.DeleteBatchJob(createdBatchJob.Name);
    }
}
