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
// [START storagetransfer_get_latest_transfer_operation]

using System;
using Google.Cloud.StorageTransfer.V1;

namespace StorageTransfer.Samples
{
    public class CheckLatestTransferOperationSample
    {
        //Checks the latest transfer operation for a given transfer job.
        public TransferJob CheckLatestTransferOperation(
             // Your Google Cloud Project ID
             string projectId = "my-project-id",
             // The name of the job to check
             string jobName = "transferJobs/1234567890")
        {
            if (string.IsNullOrEmpty(jobName))
            {
                throw new Exception("JobName can not be null or empty");
            }
            // Create a Transfer Service client
            StorageTransferServiceClient storageTransfer = StorageTransferServiceClient.Create();

            GetTransferJobRequest getTransferJobRequest = new GetTransferJobRequest { ProjectId = projectId, JobName = jobName };
            try
            {
                // Get Transfer job
                TransferJob transferJob = storageTransfer.GetTransferJob(getTransferJobRequest);
                // Get Latest operation name from transfer job
                string latestOperationName = transferJob.LatestOperationName;


                if (!string.IsNullOrEmpty(latestOperationName))
                {
                    Console.WriteLine("The latest operation for transfer job " + jobName + " is: " + latestOperationName + "");
                }
                else
                {
                    Console.WriteLine("Transfer job " + jobName + " hasn't run yet, try again after the job has started running.");
                }
                return transferJob;
            }
            catch (Exception)
            {
                throw new Exception("Failed to get transfer job " + jobName + "");
            }
        }
    }
}
// [END storagetransfer_get_latest_transfer_operation]
