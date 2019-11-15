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
<<<<<<< HEAD

=======
>>>>>>> 51eed317... Adds code snippets for AutoML Translation: create dataset, create model, predict.
using CommandLine;

namespace GoogleCloudSamples
{
<<<<<<< HEAD
    public class BaseOptions
    {
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }
    }


=======
>>>>>>> 51eed317... Adds code snippets for AutoML Translation: create dataset, create model, predict.
    [Verb("create_dataset", HelpText = "Create a new dataset for training a model")]
    public class CreateDatasetOptions
    {
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }

        [Value(1, HelpText = "Display name for your dataset")]
        public string DisplayName { get; set; }
    }

    [Verb("create_model", HelpText = "Create a new custom model")]
    public class CreateModelOptions
<<<<<<< HEAD
    {
=======
    { 
>>>>>>> 51eed317... Adds code snippets for AutoML Translation: create dataset, create model, predict.
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }

        [Value(1, HelpText = "Display name for your model")]
        public string DisplayName { get; set; }

        [Value(2, HelpText = "Name of the dataset to use for training your model")]
        public string DatasetID { get; set; }
    }

<<<<<<< HEAD
    [Verb("get_model", HelpText = "Retrieve a model for AutoML")]
    public class GetModelOptions
    {
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }

        [Value(1, HelpText = "The ID of model to retrieve.")]
        public string ModelId { get; set; }
    }

=======
>>>>>>> 51eed317... Adds code snippets for AutoML Translation: create dataset, create model, predict.
    [Verb("predict", HelpText = "Make a prediction with a custom model")]
    public class PredictOptions
    {
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }

        [Value(1, HelpText = "ID of the model to use for prediction")]
        public string ModelID { get; set; }
    }

    public class AutoMLProgram
    {
        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            AutoMLBatchPredict.RegisterCommands(verbMap);
            AutoMLListModels.RegisterCommands(verbMap);
            AutoMLDeleteModel.RegisterCommands(verbMap);
            AutoMLDeleteDataset.RegisterCommands(verbMap);
            AutoMLDeployModel.RegisterCommands(verbMap);
            AutoMLLanguageSentimentAnalysisCreateDataset.RegisterCommands(verbMap);
            AutoMLLanguageSentimentAnalysisCreateModel.RegisterCommands(verbMap);
            AutoMLLanguageSentimentAnalysisPredict.RegisterCommands(verbMap);
            AutoMLLanguageTextClassificationCreateDataset.RegisterCommands(verbMap);
            AutoMLLanguageTextClassificationCreateModel.RegisterCommands(verbMap);
            AutoMLLanguageTextClassificationPredict.RegisterCommands(verbMap);
            AutoMLLanguageEntityExtractionCreateDataset.RegisterCommands(verbMap);
            AutoMLLanguageEntityExtractionCreateModel.RegisterCommands(verbMap);
            AutoMLLanguageEntityExtractionPredict.RegisterCommands(verbMap);
            AutoMLListDatasets.RegisterCommands(verbMap);
            AutoMLListModelEvaluations.RegisterCommands(verbMap);
            AutoMLListOperationStatus.RegisterCommands(verbMap);
            AutoMLExportDataset.RegisterCommands(verbMap);
            AutoMLGetDataset.RegisterCommands(verbMap);
            AutoMLGetOperationStatus.RegisterCommands(verbMap);
            AutoMLGetModelEvaluation.RegisterCommands(verbMap);
            AutoMLGetModel.RegisterCommands(verbMap);
            AutoMLImportDataset.RegisterCommands(verbMap);
            AutoMLTranslationCreateDataset.RegisterCommands(verbMap);
            AutoMLTranslationCreateModel.RegisterCommands(verbMap);
            AutoMLTranslationPredict.RegisterCommands(verbMap);
            AutoMLUndeployModel.RegisterCommands(verbMap);
            AutoMLVisionClassificationDeployModelNodeCount.RegisterCommands(verbMap);
            AutoMLVisionClassificationCreateDataset.RegisterCommands(verbMap);
            AutoMLVisionClassificationCreateModel.RegisterCommands(verbMap);
            AutoMLVisionClassificationPredict.RegisterCommands(verbMap);
            AutoMLVisionObjectDetectionCreateDataset.RegisterCommands(verbMap);
            AutoMLVisionObjectDetectionCreateModel.RegisterCommands(verbMap);
            AutoMLVisionObjectDetectionPredict.RegisterCommands(verbMap);
            AutoMLVisionObjectDetectionDeployModelNodeCount.RegisterCommands(verbMap);

            verbMap.NotParsedFunc = (err) => 1;
            return (int)verbMap.Run(args);
        }
    }
}
