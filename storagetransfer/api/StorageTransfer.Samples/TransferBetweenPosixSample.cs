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

// [START storagetransfer_transfer_posix_to_posix]

using Google.Cloud.StorageTransfer.V1;
using System;

public class TransferBetweenPosixSample
{
    /// <summary>
    ///  Creates a transfer that transfer objects from root directory to the destination directory between POSIX file systems.
    /// </summary>
    /// <param name="projectId">The ID of the Google Cloud project.</param>
    /// <param name="sourceAgentPoolName">The agent pool associated with the POSIX data source. If not provided, defaults to the default agent.</param>
    /// <param name="sinkAgentPoolName">The agent pool associated with the POSIX data sink. If not provided, defaults to the default agent.</param>
    /// <param name="rootDirectory">The root directory path on the source filesystem.</param>
    /// <param name="destinationDirectory">The root directory path on the sink filesystem.</param>
    /// <param name="intermediateBucket">The name of GCS bucket for intermediate storage.</param>
    public TransferJob TransferBetweenPosix(
            string projectId = "my-project-id",
            string sourceAgentPoolName = "projects/my-project-id/agentPools/transfer_service_default",
            string sinkAgentPoolName = "projects/my-project-id/agentPools/transfer_service_default",
            string rootDirectory = "/tmp/uploads",
            string destinationDirectory = "/directory/to/transfer/sink",
            string intermediateBucket = "my-intermediate-bucket")
    {
        string jobDescription = $"Transfer objects from {rootDirectory} to the {destinationDirectory} between POSIX file system";

        TransferJob transferJob = new TransferJob
        {
            ProjectId = projectId,
            Description = jobDescription,
            TransferSpec = new TransferSpec
            {
                SourceAgentPoolName = sourceAgentPoolName,
                SinkAgentPoolName = sinkAgentPoolName,
                PosixDataSource = new PosixFilesystem { RootDirectory = rootDirectory },
                PosixDataSink = new PosixFilesystem { RootDirectory = destinationDirectory },
                GcsIntermediateDataLocation = new GcsData { BucketName = intermediateBucket }
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

        Console.WriteLine($"Created and ran transfer job from {rootDirectory} to {destinationDirectory} with the name {response.Name}");
        return response;
    }
}
// [END storagetransfer_transfer_posix_to_posix]
