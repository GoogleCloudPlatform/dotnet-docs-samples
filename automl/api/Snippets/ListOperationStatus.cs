// Copyright (c) 2019 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using CommandLine;
using Google.Cloud.AutoML.V1;
using Google.LongRunning;
using System;

namespace GoogleCloudSamples
{
    [Verb("list_operation_status", HelpText = "List operations")]
    public class ListOperationStatusOption : BaseOptions
    {
    }

    public class AutoMLListOperationStatus
    {
        // [START automl_list_operation_status]
        /// <summary>
        /// Demonstrates using the AutoML client to list operations.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        public static object ListOperationStatus(string projectId = "YOUR-PROJECT-ID")
        {
            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            AutoMlClient client = AutoMlClient.Create();

            // A resource that represents Google Cloud Platform location.
            string projectLocation = LocationName.Format(projectId, "us-central1");

            // Create list operations request.
            ListOperationsRequest listrequest = new ListOperationsRequest
            {
                Name = projectLocation
            };

            // List all the operations names available in the region by applying filter.
            foreach (Operation operation in
              client.CreateModelOperationsClient.ListOperations(listrequest))
            {
                Console.WriteLine($"Operation details:");
                Console.WriteLine($"\tName: {operation.Name}");
                Console.WriteLine($"\tMetadata Type Url: { operation.Metadata.TypeUrl}");
                Console.WriteLine($"\tDone: {operation.Done}");
                if (operation.Response != null)
                {
                    Console.WriteLine($"\tResponse Type Url: {operation.Response.TypeUrl}");
                }
                if (operation.Error != null)
                {
                    Console.WriteLine("\tResponse:");
                    Console.WriteLine($"\t\tError code: {operation.Error.Code}");
                    Console.WriteLine($"\t\tError message: {operation.Error.Message}");
                }
            }

            return 0;
        }

        // [END automl_list_operation_status]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((ListOperationStatusOption opts) =>
                     AutoMLListOperationStatus.ListOperationStatus(opts.ProjectID));
        }
    }
}