// Copyright 2023 Google Inc.
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
// limitations under the License

using Google.Api.Gax.ResourceNames;
using Google.Cloud.BigQuery.V2;
using Google.Cloud.Dlp.V2;
using Google.Cloud.Storage.V1;
using System.IO;
using System;
using Xunit;
using Google.Apis.Bigquery.v2.Data;

namespace GoogleCloudSamples
{
    public class DeidentifyDataStoredInCloudStorageTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public DeidentifyDataStoredInCloudStorageTests(DlpTestFixture fixture) => _fixture = fixture;

        internal static void CreateDatasetTable(string datasetId, string tableId, BigQueryClient bigQueryClient)
        {
            var dataset = new Dataset
            {
                Location = "US"
            };

            // Create the dataset.
            var datasetResponse = bigQueryClient.CreateDataset(datasetId, dataset);
            var schema = new TableSchemaBuilder
            {
                { "Name", BigQueryDbType.String },
                { "Email", BigQueryDbType.String },
            }.Build();

            // Create the table.
            datasetResponse.CreateTable(tableId, schema: schema);
        }

        public DeidentifyTemplate CreateUnstructuredTemplate(DlpServiceClient dlp)
        {
            // Construct the unstructured de-identify config.
            var unstructuredConfig = new DeidentifyConfig
            {
                InfoTypeTransformations = new InfoTypeTransformations
                {
                    Transformations =
                    {
                        new InfoTypeTransformations.Types.InfoTypeTransformation
                        {
                            InfoTypes = { new InfoType { Name = "EMAIL_ADDRESS" } },
                            PrimitiveTransformation = new PrimitiveTransformation
                            {
                                ReplaceWithInfoTypeConfig = new ReplaceWithInfoTypeConfig()
                            }
                        }
                    }
                }
            };
            var unstructuredTemplate = new DeidentifyTemplate
            {
                DisplayName = "Unstructured Template",
                DeidentifyConfig = unstructuredConfig,
            };
            var createUnstructuredTemplateRequest = new CreateDeidentifyTemplateRequest
            {
                Parent = new LocationName(_fixture.ProjectId, "global").ToString(),
                DeidentifyTemplate = unstructuredTemplate
            };
            var unstructuredResponse = dlp.CreateDeidentifyTemplate(createUnstructuredTemplateRequest);
            return unstructuredResponse;
        }

        public DeidentifyTemplate CreateStructuredTemplate(DlpServiceClient dlp)
        {
            // Construct the structured de-identify config.
            var structuredConfig = new DeidentifyConfig
            {
                RecordTransformations = new RecordTransformations
                {
                    FieldTransformations =
                    {
                        new FieldTransformation
                        {
                            Fields =
                            {
                                new FieldId { Name = "Email" }
                            },
                            InfoTypeTransformations =  new InfoTypeTransformations
                            {
                                Transformations =
                                {
                                    new InfoTypeTransformations.Types.InfoTypeTransformation
                                    {
                                        PrimitiveTransformation = new PrimitiveTransformation
                                        {
                                            ReplaceWithInfoTypeConfig = new ReplaceWithInfoTypeConfig()
                                        },
                                        InfoTypes = { new InfoType { Name = "EMAIL_ADDRESS" } },
                                    }
                                }
                            }
                        }
                    }
                }
            };
            var structuredTemplate = new DeidentifyTemplate
            {
                DisplayName = "Structured Template",
                DeidentifyConfig = structuredConfig,
            };
            var createStructuredTemplateRequest = new CreateDeidentifyTemplateRequest
            {
                Parent = new LocationName(_fixture.ProjectId, "global").ToString(),
                DeidentifyTemplate = structuredTemplate
            };
            var structuredResponse = dlp.CreateDeidentifyTemplate(createStructuredTemplateRequest);
            return structuredResponse;
        }

        public DeidentifyTemplate CreateImageRedactionTemplate(DlpServiceClient dlp)
        {
            // Create the image de-identify config.
            var imageConfig = new DeidentifyConfig
            {
                ImageTransformations = new ImageTransformations
                {
                    Transforms =
                    {
                        new ImageTransformations.Types.ImageTransformation
                        {
                            SelectedInfoTypes = new ImageTransformations.Types.ImageTransformation.Types.SelectedInfoTypes
                            {
                                InfoTypes = { new InfoType { Name = "EMAIL_ADDRESS" } }
                            }
                        }
                    }
                }
            };
            var imageRedactionTemplate = new DeidentifyTemplate
            {
                DisplayName = "Image Redaction Template",
                DeidentifyConfig = imageConfig,
            };
            var createImageTemplateRequest = new CreateDeidentifyTemplateRequest
            {
                Parent = new LocationName(_fixture.ProjectId, "global").ToString(),
                DeidentifyTemplate = imageRedactionTemplate
            };
            var imageResponse = dlp.CreateDeidentifyTemplate(createImageTemplateRequest);
            return imageResponse;
        }

        [Fact]
        public void TestDeidentify()
        {
            Random random = new Random();
            // Create the input bucket and upload the files.
            var storage = StorageClient.Create();
            var inputBucket = storage.CreateBucket(_fixture.ProjectId, $"dlp_dotnet_test_cloud_storage_{random.Next()}");

            var filePath = Path.Combine(_fixture.ResourcePath, "test.txt");
            using var fileStream = File.OpenRead(filePath);
            storage.UploadObject(inputBucket.Name, "test.txt", null, fileStream);

            // Create the output bucket.
            var outputBucket = storage.CreateBucket(_fixture.ProjectId, $"dlp_dotnet_test_cloud_storage_output_{random.Next()}");

            // Create the dataset, table and load the data inside the table.
            BigQueryClient bigQueryClient = BigQueryClient.Create(_fixture.ProjectId);
            var datasetId = $"dlp_test_dotnet_dataset_{random.Next()}";
            var tableId = $"dlp_test_dotnet_table_{random.Next()}";
            CreateDatasetTable(datasetId, tableId, bigQueryClient);

            // Create templates for de-identification.
            var dlp = DlpServiceClient.Create();
            var unstructuredTemplate = CreateUnstructuredTemplate(dlp);
            var structuredTemplate = CreateStructuredTemplate(dlp);
            var imageTemplate = CreateImageRedactionTemplate(dlp);

            try
            {
                var response = DeidentifyDataStoredInCloudStorage.Deidentify(
                    _fixture.ProjectId,
                    $"gs://{inputBucket.Name}",
                    unstructuredTemplate.Name,
                    structuredTemplate.Name,
                    imageTemplate.Name,
                    $"gs://{outputBucket.Name}",
                    datasetId,
                    tableId);

                JobsDelete.DeleteJob(response.Name);
            }
            finally
            {
                storage.DeleteBucket(inputBucket.Name, new DeleteBucketOptions { DeleteObjects = true });
                storage.DeleteBucket(outputBucket.Name, new DeleteBucketOptions { DeleteObjects = true });
                bigQueryClient.DeleteDataset(datasetId, new DeleteDatasetOptions { DeleteContents = true });
                dlp.DeleteDeidentifyTemplate(unstructuredTemplate.Name);
                dlp.DeleteDeidentifyTemplate(structuredTemplate.Name);
                dlp.DeleteDeidentifyTemplate(imageTemplate.Name);
            }
        }
    }
}