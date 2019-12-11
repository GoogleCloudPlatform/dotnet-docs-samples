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
using System;

namespace GoogleCloudSamples
{
    [Verb("vision_object_detection_deploy_model_node_count", HelpText = "Deploy a model for prediction with a specified node count (can be used to redeploy a model)")]
    public class DeployObjectDetectionModelOptions : BaseOptions
    {
        [Value(1, HelpText = "ID of model")]
        public string ModelId { get; set; }
    }

    class AutoMLVisionObjectDetectionDeployModelNodeCount
    {
        // [START automl_vision_object_detection_deploy_model_node_count]
        /// <summary>
        /// Deploy a model with a specified node count.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="modelId">the Id of the model.</param>
        public static object VisionObjectDetectionDeployModelNodeCount(string projectId = "YOUR-PROJECT-ID",
            string modelId = "YOUR-MODEL-ID")
        {
            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            AutoMlClient client = AutoMlClient.Create();

            // Get the full path of the model.
            string modelFullId = ModelName.Format(projectId, "us-central1", modelId);
            ImageObjectDetectionModelDeploymentMetadata metadata = new
                ImageObjectDetectionModelDeploymentMetadata
            {
                NodeCount = 2
            };

            DeployModelRequest request = new DeployModelRequest
            {
                Name = modelFullId,
                ImageObjectDetectionModelDeploymentMetadata = metadata
            };

            client.DeployModelAsync(request).Result.PollUntilCompleted();
            Console.WriteLine("Model deployment finished.");
            return 0;
        }
        // [END automl_vision_object_detection_deploy_model_node_count]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DeployObjectDetectionModelOptions opts) =>
                     AutoMLVisionObjectDetectionDeployModelNodeCount.VisionObjectDetectionDeployModelNodeCount(opts.ProjectID,
                                                                 opts.ModelId));
        }
    }
}