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
    class AutoMLDeployModel
    {
        [Verb("deploy_model", HelpText = "Deploy a model")]
        public class DeployModelOptions : BaseOptions
        {
            [Value(1, HelpText = "ID of model to deploy.")]
            public string ModelId { get; set; }
        }

        // [START automl_deploy_model]
        /// <summary>
        /// Deploys a model. If a model is already deployed, 
        /// deploying it with the same parameters has no effect. 
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="modelId">the Id of the model.</param>
        public static object DeployModel(string projectId = "YOUR-PROJECT-ID",
            string modelId = "YOUR-MODEL-ID")
        {
            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            AutoMlClient client = AutoMlClient.Create();

            // Get the full path of the model.
            string modelFullId = ModelName.Format(projectId, "us-central1", modelId);
            client.DeployModelAsync(modelFullId).Result.PollUntilCompleted();
            Console.WriteLine("Model deployment finished.");
            return 0;
        }

        // [END automl_deploy_model]
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DeployModelOptions opts) =>
                     AutoMLDeployModel.DeployModel(opts.ProjectID,
                                                                 opts.ModelId));
        }
    }
}
