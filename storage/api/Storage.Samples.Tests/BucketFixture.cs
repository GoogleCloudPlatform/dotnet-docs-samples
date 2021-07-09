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

using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

[CollectionDefinition(nameof(BucketFixture))]
public class BucketFixture : IDisposable, ICollectionFixture<BucketFixture>
{
    public string ProjectId { get; }
    public IList<string> TempBucketNames { get; } = new List<string>();
    public Dictionary<string, List<string>> TempBucketFiles { get; } = new Dictionary<string, List<string>>();
    public Dictionary<string, Dictionary<string, List<long>>> TempBucketArchivedFiles { get; }
        = new Dictionary<string, Dictionary<string, List<long>>>();
    public string BucketNameGeneric { get; } = Guid.NewGuid().ToString();
    public string BucketNameRegional { get; } = Guid.NewGuid().ToString();
    public string TestLocation { get; } = "us-west1";
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
        CreateBucket(BucketNameGeneric);

        // create regional bucket
        CreateRegionalBucketSample createRegionalBucketSample = new CreateRegionalBucketSample();
        createRegionalBucketSample.CreateRegionalBucket(ProjectId, BucketNameRegional, TestLocation, StorageClasses.Regional);
        SleepAfterBucketCreateUpdateDelete();
        TempBucketNames.Add(BucketNameRegional);

        //upload file to BucketName
        UploadFileSample uploadFileSample = new UploadFileSample();
        uploadFileSample.UploadFile(BucketNameGeneric, FilePath, FileName);

        Collect(FileName);
    }

    public void Dispose()
    {
        DeleteBucketSample deleteBucketSample = new DeleteBucketSample();
        DeleteFileSample deleteFileSample = new DeleteFileSample();
        DeleteFileArchivedGenerationSample deleteFileArchivedGenerationSample = new DeleteFileArchivedGenerationSample();
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

        foreach (var bucket in TempBucketArchivedFiles)
        {
            foreach (var file in bucket.Value)
            {
                foreach (var version in file.Value)
                {
                    try
                    {
                        deleteFileArchivedGenerationSample.DeleteFileArchivedGeneration(bucket.Key, file.Key, version);
                    }
                    catch (Exception)
                    {
                        // Do nothing, we delete on a best effort basis.
                    }
                }
            }
        }

        foreach (var bucketName in TempBucketNames)
        {
            try
            {
                deleteBucketSample.DeleteBucket(bucketName);
                SleepAfterBucketCreateUpdateDelete();
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
        if (!TempBucketFiles.TryGetValue(bucketName, out List<string> objectNames))
        {
            objectNames = TempBucketFiles[bucketName] = new List<string>();
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

    public void DeleteHmacKey(string accessId)
    {
        DeactivateHmacKeySample deactivateHmacKeySample = new DeactivateHmacKeySample();
        DeleteHmacKeySample deleteHmacKeySample = new DeleteHmacKeySample();

        // Deactivate key.
        deactivateHmacKeySample.DeactivateHmacKey(ProjectId, accessId);
        // Delete key.
        deleteHmacKeySample.DeleteHmacKey(ProjectId, accessId);
    }

    public void CreateBucket(string bucketName)
    {
        CreateBucketSample createBucketSample = new CreateBucketSample();
        createBucketSample.CreateBucket(ProjectId, bucketName);
        SleepAfterBucketCreateUpdateDelete();
        TempBucketNames.Add(bucketName);
    }

    /// <summary>
    /// Bucket creation/update/deletion is rate-limited. To avoid making the tests flaky, we sleep after each operation.
    /// </summary>
    internal void SleepAfterBucketCreateUpdateDelete() => Thread.Sleep(2000);

    internal string GetServiceAccountEmail()
    {
        var cred = GoogleCredential.GetApplicationDefault().UnderlyingCredential;
        switch (cred)
        {
            case ServiceAccountCredential sac:
                return sac.Id;
            // TODO: We may well need to handle ComputeCredential for Kokoro.
            default:
                throw new InvalidOperationException($"Unable to retrieve service account email address for credential type {cred.GetType()}");
        }
    }

    public void CollectArchivedFiles(string bucketName, string objectName, long? version)
    {
        if (!TempBucketArchivedFiles.TryGetValue(bucketName, out Dictionary<string, List<long>> objectNames))
        {
            objectNames = TempBucketArchivedFiles[bucketName] = new Dictionary<string, List<long>>();
        }

        if (!objectNames.TryGetValue(objectName, out List<long> versions))
        {
            versions = objectNames[objectName] = new List<long>();
        }
        versions.Add(version.Value);
    }
}
