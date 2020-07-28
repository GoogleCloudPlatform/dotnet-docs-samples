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

using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using Xunit;

[CollectionDefinition(nameof(BucketFixture))]
public class BucketFixture : IDisposable, ICollectionFixture<BucketFixture>
{
    public string ProjectId { get; }
    public IList<string> TempBucketNames { get; } = new List<string>();
    public SortedDictionary<string, SortedSet<string>> TempBucketFiles { get; } = new SortedDictionary<string, SortedSet<string>>();
    public string BucketNameGeneric { get; } = Guid.NewGuid().ToString();
    public string BucketNameRegional { get; } = Guid.NewGuid().ToString();
    public string FileName { get; } = "Hello.txt";
    public string FilePath { get; } = "Resources/Hello.txt";
    public string KmsKeyRing { get; } = Environment.GetEnvironmentVariable("STORAGE_KMS_KEYRING");
    public string KmsKeyName { get; } = Environment.GetEnvironmentVariable("STORAGE_KMS_KEYNAME");
    public string KmsKeyLocation { get; } = "us-west1";
    public string ServiceAccountEmail { get; } = "gcs-iam-acl-test@dotnet-docs-samples-tests.iam.gserviceaccount.com";

    public BucketFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrWhiteSpace(ProjectId))
        {
            throw new Exception("You need to set the Environment variable 'GOOGLE_PROJECT_ID' with your Google Cloud Project's project id.");
        }
        // create simple bucket
        CreateBucketSample createBucketSample = new CreateBucketSample();
        createBucketSample.CreateBucket(ProjectId, BucketNameGeneric);
        TempBucketNames.Add(BucketNameGeneric);

        // create regional bucket
        CreateRegionalBucketSample createRegionalBucketSample = new CreateRegionalBucketSample();
        createRegionalBucketSample.CreateRegionalBucket(ProjectId, BucketNameRegional, KmsKeyLocation, StorageClasses.Regional);
        TempBucketNames.Add(BucketNameRegional);

        //upload file to BucketName
        UploadFileSample uploadFileSample = new UploadFileSample();
        uploadFileSample.UploadFile(BucketNameGeneric, FilePath);

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
                try
                {
                    deleteFileSample.DeleteFile(bucket.Key, file);
                }
                catch (Exception)
                {
                    // Do nothing, we delete on a best effort basis.
                }
            }
        }

        foreach (var bucketName in TempBucketNames)
        {
            try
            {
                deleteBucketSample.DeleteBucket(bucketName);
            }
            catch (Exception)
            {
                // Do nothing, we delete on a best effort basis.
            }
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
    public string Collect(string objectName) => Collect(BucketNameGeneric, objectName);

    /// <summary>
    /// Add a object located in a regional bucket to delete
    /// at the end of the test.
    /// </summary>
    /// <returns>The regional objectName.</returns>
    public string CollectRegionalObject(string objectName) => Collect(BucketNameRegional, objectName);
}
