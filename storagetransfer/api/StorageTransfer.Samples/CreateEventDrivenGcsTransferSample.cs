/**
 * Copyright 2024 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START storagetransfer_create_event_driven_gcs_transfer]

using System;
using Google.Cloud.StorageTransfer.V1;

    public class CreateEventDrivenGcsTransferSample
    {
        public TransferJob CreateEventDrivenGcsTransfer(
            // Your Google Cloud Project ID
            string projectId = "my-project-id",
            // The GCS bucket to transfer data from
            string sourceBucket = "my-source-bucket",
            // The GCS bucket to transfer data to
            string sinkBucket = "my-sink-bucket",
            // The subscription ID to a Pubsub queue to track
            string pubSubId = "projects/PROJECT_NAME/subscriptions/SUBSCRIPTION_ID")
        {
            // A useful description for your transfer job
            string jobDescription = $"Event driven gcs data transfer from {sourceBucket} to {sinkBucket} subscribed to {pubSubId} ";

            TransferJob transferJob = new TransferJob
            {
                ProjectId = projectId,
                Description = jobDescription,
                TransferSpec = new TransferSpec
                {
                    GcsDataSink = new GcsData { BucketName = sinkBucket },
                    GcsDataSource = new GcsData { BucketName = sourceBucket },
                },
                Status = TransferJob.Types.Status.Enabled,
                EventStream = new EventStream { Name = pubSubId }
            };

            StorageTransferServiceClient client = StorageTransferServiceClient.Create();
            TransferJob response = client.CreateTransferJob(new CreateTransferJobRequest { TransferJob = transferJob });
            Console.WriteLine($"Created an event driven transfer job from {sourceBucket} to {sinkBucket} subscribed to {pubSubId} with name {response.Name}");
            return response;
        }
    }
// [END storagetransfer_create_event_driven_gcs_transfer]

