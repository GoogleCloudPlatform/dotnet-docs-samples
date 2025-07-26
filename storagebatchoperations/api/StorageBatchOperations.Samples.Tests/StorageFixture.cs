// Copyright 2025 Google LLC
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

using Google.Api.Gax.ResourceNames;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Google.Cloud.StorageBatchOperations.V1;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

[CollectionDefinition(nameof(StorageFixture))]
public class StorageFixture : IDisposable, ICollectionFixture<StorageFixture>
{
    public string ProjectId { get; }
    public string LocationId { get; } = "global";
    public IList<string> TempBucketNames { get; } = [];
    public string ServiceAccountEmail { get; } = "gcs-iam-acl-test@dotnet-docs-samples-tests.iam.gserviceaccount.com";
    public StorageClient Client { get; }
    public StorageBatchOperationsClient OperationsClient { get; }
    public LocationName LocationName { get; }

    public StorageFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrWhiteSpace(ProjectId))
        {
            throw new Exception("You need to set the Environment variable 'GOOGLE_PROJECT_ID' with your Google Cloud Project's project id.");
        }
        LocationName = LocationName.FromProjectLocation(ProjectId, LocationId);
        Client = StorageClient.Create();
        OperationsClient = StorageBatchOperationsClient.Create();
    }

    /// <summary>
    /// Creates a bucket.
    /// </summary>
    /// <returns>A bucket.</returns>
    internal Bucket CreateBucket(string name, bool multiVersion, bool softDelete = false, bool registerForDeletion = true)
    {
        var bucket = Client.CreateBucket(ProjectId,
            new Bucket
            {
                Name = name,
                Versioning = new Bucket.VersioningData { Enabled = multiVersion },
                // The minimum allowed for soft delete is 7 days.
                SoftDeletePolicy = softDelete ? new Bucket.SoftDeletePolicyData { RetentionDurationSeconds = (int) TimeSpan.FromDays(7).TotalSeconds } : null,
            });
        SleepAfterBucketCreateUpdateDelete();
        if (registerForDeletion)
        {
            TempBucketNames.Add(name);
        }
        return bucket;
    }

    /// <summary>
    /// Generate the name of the bucket.
    /// </summary>
    /// <returns>The bucketName.</returns>
    internal string GenerateBucketName() => Guid.NewGuid().ToString();

    /// <summary>
    /// Generates a new globally unique identifier (GUID).
    /// </summary>
    /// <returns>A new randomly generated GUID as string.</returns>
    internal string GenerateGuid() => Guid.NewGuid().ToString();

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

    /// <summary>
    /// Deletes the batch job at the end of the test.
    /// </summary>
    internal void DeleteBatchJob(string jobName) => OperationsClient.DeleteJob(jobName);

    public void Dispose()
    {
        foreach (var bucketName in TempBucketNames)
        {
            try
            {
                Client.DeleteBucket(bucketName, new DeleteBucketOptions { DeleteObjects = true });
                SleepAfterBucketCreateUpdateDelete();
            }
            catch (Exception)
            {
                // Do nothing, we delete on a best effort basis.
            }
        }
    }
}
