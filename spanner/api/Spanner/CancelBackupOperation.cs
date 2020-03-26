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

using Google.LongRunning;
using log4net;
using static GoogleCloudSamples.Spanner.Program;

namespace GoogleCloudSamples.Spanner
{
    public class CancelBackupOperation
    {
        static readonly ILog s_logger = LogManager.GetLogger(typeof(CancelBackupOperation));

        // [START spanner_cancel_backup_create]
        public static object SpannerCancelBackupOperation(string operationName)
        {
            OperationsClient operationsClient = OperationsClient.Create();

            // Initialize Cancel operation Request instance
            var cancelOperationRequest = new CancelOperationRequest
            {
                Name = operationName
            };

            operationsClient.CancelOperation(cancelOperationRequest);

            s_logger.Info($"operation {operationName} canceled.");

            return ExitCode.Success;
        }
        // [END spanner_cancel_backup_create]
    }
}
