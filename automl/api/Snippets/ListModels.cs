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
    [Verb("list_model", HelpText = "List all models")]
    public class ListModelOptions
    {
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }
    }

    class AutoMLListModels
    {
        // [START automl_list_models]
        /// <summary>
        /// Demonstrates using the AutoML client to list all models.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        public static object ListModels(string projectId = "YOUR-PROJECT-ID")
        {
            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            AutoMlClient client = AutoMlClient.Create();

            // A resource that represents Google Cloud Platform location.
            string projectLocation = LocationName.Format(projectId, "us-central1");

            // Create list models request.
            ListModelsRequest listModlesRequest = new ListModelsRequest
            {
                Parent = projectLocation,
                Filter = ""
            };

            // List all the models available in the region by applying filter.
            Console.WriteLine("List of models:");
            foreach (Model model in client.ListModels(listModlesRequest))
            {
                // Display the model information.
                Console.WriteLine($"Model name: {model.Name}");
                // To get the model id, you have to parse it out of the `name` field. As models Ids are
                // required for other methods.
                // Name Format: `projects/{project_id}/locations/{location_id}/models/{model_id}`
                string[] names = model.Name.Split("/");
                string retrievedModelId = names[names.Length - 1];
                Console.WriteLine($"Model id: {retrievedModelId}");
                Console.WriteLine($"Model display name: {model.DisplayName}");
                Console.WriteLine("Model create time:");
                Console.WriteLine($"\tseconds: {model.CreateTime.Seconds}");
                Console.WriteLine($"\tnanos: {model.CreateTime.Nanos}");
                Console.WriteLine($"Model deployment state: {model.DeploymentState}");
            }

            return 0;
        }

        // [END automl_list_models]
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((ListModelOptions opts) =>
                     AutoMLListModels.ListModels(opts.ProjectID));
        }
    }
}