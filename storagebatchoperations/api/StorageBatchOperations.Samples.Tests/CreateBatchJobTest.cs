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
    private string _kmsKey;
    private string _keyRingId;
    private string _cryptoKeyId;
    private CryptoKeyName _cryptoKeyName;

    public CreateBatchJobTest(StorageFixture fixture)
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

    [Theory]
    [InlineData("DeleteObject")]
    [InlineData("PutObjectHold")]
    [InlineData("RewriteObject")]
    [InlineData("PutMetadata")]
    public void TestCreateBatchJob(string jobTransformationCase)
    {
        CreateBatchJobSample createJob = new CreateBatchJobSample();
        var jobId = _fixture.GenerateGuid();
        var holdState = "EventBasedHoldSet";
        var jobTransformationObject = new object();
        string jobType;

        // If the job transformation case is PutObjectHold, we can set the hold state to EventBasedHoldSet or EventBasedHoldUnSet or TemporaryHoldSet or TemporaryHoldUnSet.
        if (jobTransformationCase == "PutObjectHold")
        {
            jobType = $"{jobTransformationCase}{holdState}";
        }
        // If the job transformation case is other than PutObjectHold, we dont set the hold state.
        else
        {
            jobType = jobTransformationCase;
        }
        // If the job transformation case is RewriteObject, we can set the KmsKey and KmsKeyAsCryptoKeyName.
        if (jobTransformationCase == "RewriteObject")
        {
            string kmsKeyName = $"projects/{_fixture.ProjectId}/locations/{_fixture.LocationId}/keyRings/{_fixture.KmsKeyRing}/cryptoKeys/{_fixture.KmsKeyName}";
            var cryptoKeyName = CryptoKeyName.FromProjectLocationKeyRingCryptoKey(_fixture.ProjectId, _fixture.LocationId, _fixture.KmsKeyRing, _fixture.KmsKeyName);
            RewriteObject rewriteObject = new RewriteObject { KmsKey = kmsKeyName, KmsKeyAsCryptoKeyName = cryptoKeyName };
            jobTransformationObject = rewriteObject;

        }
        // If the job transformation case is PutMetadata, we can set the CacheControl, ContentDisposition, ContentEncoding, ContentLanguage, ContentType and CustomTime.
        else if (jobTransformationCase == "PutMetadata")
        {
            PutMetadata putMetadata = new PutMetadata
            {
                CacheControl = "no-cache",
                ContentDisposition = "inline",
                ContentEncoding = "gzip",
                ContentLanguage = "en-US",
                ContentType = "text/plain",
                CustomTime = DateTime.UtcNow.ToString("o")
            };
            jobTransformationObject = putMetadata;
        }
        // Create a batch job with the specified transformation case and bucket list
        var createdBatchJob = createJob.CreateBatchJob(_fixture.LocationName, _bucketList, jobId, jobType, jobTransformationObject);
        Assert.Equal(createdBatchJob.BucketList, _bucketList);
        Assert.Equal(createdBatchJob.TransformationCase.ToString(), jobTransformationCase);
        Assert.Equal(createdBatchJob.SourceCase.ToString(), _bucketList.GetType().Name);
        Assert.NotNull(createdBatchJob.Name);
        Assert.NotNull(createdBatchJob.JobName);
        Assert.NotNull(createdBatchJob.CreateTime);
        Assert.NotNull(createdBatchJob.CompleteTime);
        _fixture.DeleteBatchJob(createdBatchJob.Name);
    }
}
