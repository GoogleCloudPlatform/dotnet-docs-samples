using CommandLine;
using Google.Cloud.AutoML.V1;
using System;
using System.IO;

namespace GoogleCloudSamples
{

    public class AutoMLDeleteModel
    {
        [Verb("delete_model", HelpText = "Delete a custommodel")]
        public class DeleteModelOptions : PredictOptions
        {
            [Value(0, HelpText = "Your project ID")]
            public string ProjectID { get; set; }

            [Value(1, HelpText = "ID of the model to use for prediction")]
            public string ModelID { get; set; }
        }

        // [START automl_delete_model]
        /// <summary>
        /// Deletes a custom AutoML model.
        /// </summary>
        /// <returns>Success or failure as integer</returns>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="modelId">Model identifier.</param>
        public static object DeleteModel(string projectId,
                                         string modelId)
        {

            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            var client = AutoMlClient.Create();

            // Get the full path of the model.
            var modelFullName =
                ModelName.Format(projectId, "us-central1", modelId);

            // Delete the model
            var response = client.DeleteModel(modelFullName);

            Console.WriteLine("Model deletion started ...");
            Console.WriteLine($"Model deleted. {response}");

            return 0;

        }
        // [END automl_delete_model]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DeleteModelOptions opts) =>
                     AutoMLDeleteModel.DeleteModel(opts.ProjectID,
                                                   opts.ModelID));
        }
    }
}
