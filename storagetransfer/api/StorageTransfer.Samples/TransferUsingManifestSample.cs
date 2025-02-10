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

// [START storagetransfer_manifest_request]

using System;
using Google.Cloud.StorageTransfer.V1;

public class TransferUsingManifestSample
{
    // Create a transfer from a POSIX file system to a GCS bucket using a manifest file
    public TransferJob TransferUsingManifest(
        // Your Google Cloud Project ID
        string projectId = "my-project-id",
        // The agent pool associated with the POSIX data source. If not provided, defaults to the default agent
        string sourceAgentPoolName = "projects/my-project-id/agentPools/transfer_service_default",
        // The root directory path on the source filesystem
        string rootDirectory = "/tmp/uploads",
        // The GCS bucket which has your manifest file
        string manifestBucket = "my-source-bucket",
        // The GCS bucket to transfer data to
        string sinkBucket = "my-sink-bucket",
        // The name of the manifest file in manifestBucket that specifies which objects to transfer
        string manifestObjectName = "path/to/manifest.csv")
    {
        string manifestLocation = $"gs://{manifestBucket}/{manifestObjectName}";

        // A useful description for your transfer job
        string jobDescription = $"Transfers objects from a POSIX file system to a sink bucket ({sinkBucket}) using manifest file";

        TransferJob transferJob = new TransferJob
        {
            ProjectId = projectId,
            Description = jobDescription,
            TransferSpec = new TransferSpec
            {
                GcsDataSink = new GcsData { BucketName = sinkBucket },
                GcsDataSource = new GcsData { BucketName = manifestBucket },
                SourceAgentPoolName = sourceAgentPoolName,
                PosixDataSource = new PosixFilesystem { RootDirectory = rootDirectory },
                TransferManifest = new TransferManifest { Location = manifestLocation }
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

        Console.WriteLine($"Created and ran transfer job from {rootDirectory} to {sinkBucket} using manifest file {manifestLocation} with the name {response.Name}");
        return response;
    }
}
// [END storagetransfer_manifest_request]
