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

// [START storagetransfer_download_to_posix]

using Google.Cloud.StorageTransfer.V1;
using System;

public class DownloadToPosixSample
{
    /// <summary>
    /// Creates a transfer for downloading objects from a gcs bucket to the root directory of POSIX file system.
    /// </summary>
    /// <param name="projectId">The ID of the Google Cloud project.</param>
    /// <param name="sinkAgentPoolName">The agent pool associated with the POSIX data sink. If not provided, defaults to the default agent.</param>
    /// <param name="sourceBucket">Your GCS source bucket name.</param>
    /// <param name="gcsSourcePath">An optional path on the Google Cloud Storage bucket to download from.</param>
    /// <param name="rootDirectory">The root directory path on the source filesystem.</param>
    public TransferJob DownloadToPosix(
        string projectId = "my-project-id",
        string sinkAgentPoolName = "projects/my-project-id/agentPools/transfer_service_default",
        string sourceBucket = "my-source-bucket",
        string gcsSourcePath = "foo/bar/",
        string rootDirectory = "/tmp/uploads")
    {
        string jobDescription = $"Download objects from a GCS source bucket ({sourceBucket}/{gcsSourcePath}) to the root directory of POSIX file system";

        TransferJob transferJob = new TransferJob
        {
            ProjectId = projectId,
            Description = jobDescription,
            TransferSpec = new TransferSpec
            {
                GcsDataSource = new GcsData { BucketName = sourceBucket, Path = gcsSourcePath },
                SinkAgentPoolName = sinkAgentPoolName,
                PosixDataSink = new PosixFilesystem { RootDirectory = rootDirectory }
            },
            Status = TransferJob.Types.Status.Enabled,
        };

        StorageTransferServiceClient client = StorageTransferServiceClient.Create();
        TransferJob response = client.CreateTransferJob(new CreateTransferJobRequest { TransferJob = transferJob });
        client.RunTransferJob(new RunTransferJobRequest
        {
            JobName = response.Name,
            ProjectId = projectId
        });

        Console.WriteLine($"Created and ran transfer job from ({sourceBucket}/{gcsSourcePath}) to {rootDirectory} with the name {response.Name}");
        return response;
    }
}
// [END storagetransfer_download_to_posix]
