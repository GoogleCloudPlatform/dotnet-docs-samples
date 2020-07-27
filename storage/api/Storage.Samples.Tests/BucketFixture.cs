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
using System.Collections.Generic;
using Xunit;

[CollectionDefinition(nameof(BucketFixture))]
public class BucketFixture : IDisposable, ICollectionFixture<BucketFixture>
{
    public readonly string ProjectId;
    public List<string> TempBucketNames { get; set; } = new List<string>();
    public SortedDictionary<string, SortedSet<string>> TempBucketFiles { get; set; } = new SortedDictionary<string, SortedSet<string>>();
    public string BucketName { get; private set; } = Guid.NewGuid().ToString();
    public string BucketName1 { get; private set; } = Guid.NewGuid().ToString();
    public string FileName { get; private set; } = "Hello.txt";
    public string FilePath { get; private set; } = "Resources/Hello.txt";
    public string KmsKeyRing { get; private set; } = Environment.GetEnvironmentVariable("STORAGE_KMS_KEYRING");
    public string KmsKeyName { get; private set; } = Environment.GetEnvironmentVariable("STORAGE_KMS_KEYNAME");
    public string KmsKeyLocation { get; private set; } = "us-west1";
    public string ServiceAccountEmail { get; private set; } = "gcs-iam-acl-test@dotnet-docs-samples-tests.iam.gserviceaccount.com";

    public BucketFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        // create simple bucket
        CreateBucketSample createBucketSample = new CreateBucketSample();
        createBucketSample.CreateBucket(ProjectId, BucketName);
        TempBucketNames.Add(BucketName);

        // create regional bucket
        CreateRegionalBucketSample createRegionalBucketSample = new CreateRegionalBucketSample();
        createRegionalBucketSample.CreateRegionalBucket(ProjectId, KmsKeyLocation, BucketName1);
        TempBucketNames.Add(BucketName1);

        //upload file to BucketName
        UploadFileSample uploadFileSample = new UploadFileSample();
        uploadFileSample.UploadFile(BucketName, FilePath);

        Collect(FileName);
    }
    public void Dispose()
    {
        DeleteBucketSample deleteBucketSample = new DeleteBucketSample();
        DeleteFileSample deleteFileSample = new DeleteFileSample();
        foreach (var bucket in TempBucketFiles)
        {
            foreach (var file in bucket.Value)
            {
                deleteFileSample.DeleteFile(bucket.Key, file);
            }
        }

        foreach (var bucketName in TempBucketNames)
        {
            deleteBucketSample.DeleteBucket(bucketName);
        }
    }

    /// <summary>
    /// Add an object to delete at the end of the test.
    /// </summary>
    /// <returns>The objectName.</returns>
    private string Collect(string bucketName, string objectName)
    {
        SortedSet<string> objectNames;
        if (!TempBucketFiles.TryGetValue(bucketName, out objectNames))
        {
            objectNames = TempBucketFiles[bucketName] = new SortedSet<string>();
        }
        objectNames.Add(objectName);
        return objectName;
    }

    /// <summary>
    /// Add an object to delete at the end of the test.
    /// </summary>
    /// <returns>The objectName.</returns>
    public string Collect(string objectName) => Collect(BucketName, objectName);

    /// <summary>
    /// Add a object located in a regional bucket to delete
    /// at the end of the test.
    /// </summary>
    /// <returns>The regional objectName.</returns>
    public string CollectRegionalObject(string objectName) => Collect(BucketName1, objectName);
}
