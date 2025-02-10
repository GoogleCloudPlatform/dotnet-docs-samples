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

using System;
using Google.Cloud.StorageTransfer.V1;

public class DownloadToPosixSample
{
    // Create a transfer from a GCS bucket to a POSIX file system
    public TransferJob DownloadToPosix(
        // Your Google Cloud Project ID
        string projectId = "my-project-id",
        // The agent pool associated with the POSIX data sink. If not provided, defaults to the default agent
        string sinkAgentPoolName = "projects/my-project-id/agentPools/transfer_service_default",
        // Your GCS source bucket name
        string gcsSourceBucket = "my-gcs-source-bucket",
        // An optional path on the Google Cloud Storage bucket to download from
        string gcsSourcePath = "foo/bar/",
        // The root directory path on the source filesystem
        string rootDirectory = "/tmp/uploads")
    {
        // A useful description for your transfer job
        string jobDescription = $"Download objects from a GCS source bucket ({gcsSourceBucket}/{gcsSourcePath}) to the root directory of POSIX file system";

        TransferJob transferJob = new TransferJob
        {
            ProjectId = projectId,
            Description = jobDescription,
            TransferSpec = new TransferSpec
            {
                GcsDataSource = new GcsData { BucketName = gcsSourceBucket, Path = gcsSourcePath },
                SinkAgentPoolName = sinkAgentPoolName,
                PosixDataSink = new PosixFilesystem { RootDirectory = rootDirectory }
            },
            Status = TransferJob.Types.Status.Enabled,
        };

        // Create a Transfer Service client
        StorageTransferServiceClient client = StorageTransferServiceClient.Create();

        // Create a Transfer job
        TransferJob response = client.CreateTransferJob(new CreateTransferJobRequest { TransferJob = transferJob });

        client.RunTransferJob(new RunTransferJobRequest
        {
            JobName = response.Name,
            ProjectId = projectId
        });

        Console.WriteLine($"Created and ran transfer job from ({gcsSourceBucket}/{gcsSourcePath}) to {rootDirectory} with the name {response.Name}");
        return response;
    }
}
//[END storagetransfer_download_to_posix]


