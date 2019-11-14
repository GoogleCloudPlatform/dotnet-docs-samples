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

// [START automl_get_model]

using Google.Cloud.AutoML.V1;
using System;

class AutoMLGetModel
{
    /// <summary>
    /// Demonstrates using the AutoML client to get a model by ID.
    /// </summary>
    /// <param name="projectId">GCP Project ID.</param>
    /// <param name="modelId">the Id of the model.</param>
    public static void GetModel(string projectId = "YOUR-PROJECT-ID",
        string modelId = "YOUR-MODEL-ID")
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests. After completing all of your requests, call
        // the "close" method on the client to safely clean up any remaining background resources.
        AutoMlClient client = AutoMlClient.Create();

        // Get the full path of the model.
        string modelFullId = ModelName.Format(projectId, "us-central1", modelId);
        Model model = client.GetModel(modelFullId);

        // Display the model information.
        Console.WriteLine($"Model name: {model.Name}");
        // To get the model id, you have to parse it out of the `name` field. As models Ids are
        // required for other methods.
        // Name Format: `projects/{project_id}/locations/{location_id}/models/{model_id}`
        string[] names = model.Name.Split("/");
        string retrievedModelId = names[names.Length - 1];
        Console.WriteLine($"Model id: {retrievedModelId}");
        Console.WriteLine($"Model display name: {model.DisplayName}");
        Console.WriteLine($"Model create time:");
        Console.WriteLine($"\tseconds: { model.CreateTime.Seconds}");
        Console.WriteLine($"\tnanos: {model.CreateTime.Nanos}");
        Console.WriteLine($"Model deployment state: { model.DeploymentState}");
    }

    public static int Main(string[] args)
    {
        //...
        AutoMLLanguageEntityExtractionPredict.LanguageEntityExtractionPredict("python-docs-samples-tests",
            "TEN5112482778553778176",
            "Haploinsufficiency of the transcription factors FOXC1 and FOXC2 results in aberrant ocular development.\tAnterior segment developmental disorders , including Axenfeld-Rieger anomaly ( ARA ) , variably associate with harmfully elevated intraocular pressure ( IOP ) , which causes glaucoma . Clinically observed dysgenesis does not correlate with IOP , however , and the etiology of glaucoma development is not understood . The forkhead transcription factor genes Foxc1 ( formerly Mf1 ) and Foxc2 ( formerly Mfh1 ) are expressed in the mesenchyme from which the ocular drainage structures derive . Mutations in the human homolog of Foxc1 , FKHL7 , cause dominant anterior segment defects and glaucoma in various families . We show that Foxc1 ( + / - ) mice have anterior segment abnormalities similar to those reported in human patients . These abnormalities include small or absent Schlemms canal , aberrantly developed trabecular meshwork , iris hypoplasia , severely eccentric pupils and displaced Schwalbes line . The penetrance of clinically obvious abnormalities varies with genetic background . In some affected eyes , collagen bundles were half normal diameter , or collagen and elastic tissue were very sparse . Thus , abnormalities in extracellular matrix synthesis or organization may contribute to development of the ocular phenotypes . Despite the abnormalities in ocular drainage structures in Foxc1 ( + / - ) mice , IOP was normal in almost all mice analyzed , on all genetic backgrounds and at all ages . Similar abnormalities were found in Foxc2 ( + / - ) mice , but no disease-associated mutations were identified in the human homolog FKHL14 in 32 ARA patients . Foxc1 ( + / - ) and Foxc2 ( + / - ) mice are useful models for studying anterior segment development and its anomalies , and may allow identification of genes that interact with Foxc1 and Foxc2 ( or FKHL7 and FKHL14 ) to produce a phenotype with elevated IOP and glaucoma . .");
        return 0;
    }
}


// [END automl_get_model]