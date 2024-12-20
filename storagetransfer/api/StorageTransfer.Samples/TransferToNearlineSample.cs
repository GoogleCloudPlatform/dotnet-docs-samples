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
// [START storagetransfer_transfer_to_nearline]

using System;
using Google.Cloud.StorageTransfer.V1;
using Google.Protobuf.WellKnownTypes;

    public class TransferToNearlineSample
    {
        //Creates a one-off transfer job that transfers objects from a standard GCS bucket that are more
        // than 30 days old to a Nearline GCS bucket.
        public TransferJob TransferToNearline(
            // Your Google Cloud Project ID
            string projectId = "my-project-id",
            // The GCS bucket to transfer objects from
            string sourceBucket = "my-source-bucket",
            // The GCS Nearline bucket to transfer old objects to
            string sinkBucket = "my-sink-bucket")
        {
            // A description of this job
            string jobDescription = $"Transfers old objects from standard bucket ({sourceBucket}) that haven't been modified in the last 30 days to a Nearline bucket ({sinkBucket})";

            TransferJob transferJob = new TransferJob
            {
                ProjectId = projectId,
                Description = jobDescription,
                TransferSpec = new TransferSpec
                {
                    GcsDataSink = new GcsData { BucketName = sinkBucket },
                    GcsDataSource = new GcsData { BucketName = sourceBucket },
                    ObjectConditions = new ObjectConditions { MinTimeElapsedSinceLastModification = Duration.FromTimeSpan(TimeSpan.FromSeconds(2592000)) },
                    TransferOptions = new TransferOptions { DeleteObjectsFromSourceAfterTransfer = true },
                },
                Status = TransferJob.Types.Status.Enabled,
                Schedule = new Schedule { ScheduleStartDate = Google.Type.Date.FromDateTime(System.DateTime.UtcNow.Date.AddMonths(1)), ScheduleEndDate = Google.Type.Date.FromDateTime(System.DateTime.UtcNow.Date.AddMonths(1)) }
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

            Console.WriteLine($"Created one-off transfer job from standard bucket {sourceBucket} to Nearline bucket {sinkBucket} with the name {response.Name}");
            return response;
        }
    }

// [END storagetransfer_transfer_to_nearline]
