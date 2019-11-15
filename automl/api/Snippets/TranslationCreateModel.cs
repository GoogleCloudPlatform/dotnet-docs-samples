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
    [Verb("create_translation_model", HelpText = "Create a new custom AutoML Translation model")]
    public class AutoMLTranslationCreateModelOptions : CreateModelOptions
    { }

    public class AutoMLTranslationCreateModel
    {
        // [START automl_translate_create_model]
        /// <summary>
        /// Creates a new model for AutoML Translation
        /// </summary>
        /// <returns>Success or failure as integer</returns>
        /// <param name="projectID">GCP Project ID.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="datasetID">Dataset identifier.</param>
        public static object TranslationCreateModel(string projectID,
                                                    string displayName,
                                                    string datasetID)
        {

            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            var client = AutoMlClient.Create();

            // A resource that represents Google Cloud Platform location.
            var locationName = LocationName.Format(projectID, "us-central1");

            var modelRequest = new Model
            {
                DisplayName = displayName,
                DatasetId = datasetID,
                TranslationModelMetadata = new TranslationModelMetadata()
            };

            // Create a model with the model metadata in the region.
            var response = client.CreateModel(locationName, modelRequest);

            // Don't wait for model creation to finish, as this can take several hours.
            // However, you can use the `name` of the operation to check the status of your model.

            Console.WriteLine($"Training operation name: {response.Name}");
            Console.WriteLine("Training started...");

            return 0;
        }
        // [END automl_translate_create_model]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((AutoMLTranslationCreateModelOptions opts) =>
                     AutoMLTranslationCreateModel.TranslationCreateModel(opts.ProjectID,
                                                                         opts.DisplayName,
                                                                         opts.DatasetID));
        }
    }
}
