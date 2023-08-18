// Copyright 2023 Google Inc.
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

// [START dlp_deidentify_cloud_storage]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using System.Linq;

public class DeidentifyDataStoredInCloudStorage
{
    public static DlpJob Deidentify(
        string projectId,
        string gcsInputPath,
        string unstructuredDeidentifyTemplatePath,
        string structuredDeidentifyTemplatePath,
        string imageRedactionTemplatePath,
        string gcsOutputPath,
        string datasetId,
        string tableId)
    {
        // Instantiate the client.
        var dlp = DlpServiceClient.Create();

        //Construct the storage config by specifying the input directory.
        var storageConfig = new StorageConfig
        {
            CloudStorageOptions = new CloudStorageOptions
            {
                FileSet = new CloudStorageOptions.Types.FileSet
                {
                    Url = gcsInputPath
                }
            }
        };

        // Construct the inspect config by specifying the type of info to be inspected.
        var inspectConfig = new InspectConfig
        {
            InfoTypes =
            {
                new InfoType[]
                {
                    new InfoType { Name = "PERSON_NAME" },
                    new InfoType { Name = "EMAIL_ADDRESS" }
                }
            },
            IncludeQuote = true
        };

        // Construct the actions to take after the inspection portion of the job is completed.
        // Specify how Cloud DLP must de-identify sensitive data in structured files, unstructured files and images
        // using Transformation config.
        // The de-identified files will be written to the the GCS bucket path specified in gcsOutputPath and the details of 
        // transformations performed will be written to BigQuery table specified in datasetId and tableId.
        var actions = new Action[]
        {
            new Action
            {
                Deidentify = new Action.Types.Deidentify
                {
                    CloudStorageOutput = gcsOutputPath,
                    TransformationConfig = new TransformationConfig
                    {
                        DeidentifyTemplate = unstructuredDeidentifyTemplatePath,
                        ImageRedactTemplate = imageRedactionTemplatePath,
                        StructuredDeidentifyTemplate = structuredDeidentifyTemplatePath,
                    },
                    TransformationDetailsStorageConfig = new TransformationDetailsStorageConfig
                    {
                        Table = new BigQueryTable
                        {
                            ProjectId = projectId,
                            DatasetId = datasetId,
                            TableId = tableId
                        }
                    }
                }
            }
        };

        // Construct the inspect job config using created storage config, inspect config and actions.
        var inspectJob = new InspectJobConfig
        {
            StorageConfig = storageConfig,
            InspectConfig = inspectConfig,
            Actions = { actions }
        };

        // Create the dlp job and call the API.
        DlpJob response = dlp.CreateDlpJob(new CreateDlpJobRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            InspectJob = inspectJob
        });

        return response;
    }
}

// [END dlp_deidentify_cloud_storage]
