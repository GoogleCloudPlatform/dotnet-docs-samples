// Copyright 2021 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START retail_import_products_from_gcs]
// Import products into a catalog from gcs using Retail API

using Google.Cloud.Retail.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace grs_product
{
    public static class ImportProductsGcs
    {
        private const string Endpoint = "retail.googleapis.com";

        private const string gcsProductsObject = "products.json";

        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string BucketName = Environment.GetEnvironmentVariable("BUCKET_NAME");

        private static readonly string DefaultCatalog = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog/branches/default_branch";
        private static readonly string gcsBucket = $"gs://{BucketName}";
        private static readonly string gcsErrorsBucket = $"{gcsBucket}/error";

        // TO CHECK ERROR HANDLING USE THE JSON WITH INVALID PRODUCT
        // gcs_products_object = "products_some_invalid.json"

        // Get product service client
        private static ProductServiceClient GetProductServiceClient()
        {
            var productServiceClientBuilder = new ProductServiceClientBuilder
            {
                Endpoint = Endpoint
            };

            var productServiceClient = productServiceClientBuilder.Build();
            return productServiceClient;
        }

        // Get import products gcs request
        private static ImportProductsRequest GetImportProductsGcsRequest(string gcsObjectName)
        {
            // TO CHECK ERROR HANDLING PASTE THE INVALID CATALOG NAME HERE:
            // DefaultCatalog = "invalid_catalog_name"
            var gcsSource = new GcsSource();
            gcsSource.InputUris.Add($"{gcsBucket}/{gcsObjectName}");

            var inputConfig = new ProductInputConfig
            {
                GcsSource = gcsSource
            };

            Console.WriteLine("\nGCS source: \n" + gcsSource.InputUris);

            var errorsConfig = new ImportErrorsConfig
            {
                GcsPrefix = gcsErrorsBucket
            };

            var importRequest = new ImportProductsRequest
            {
                Parent = DefaultCatalog,
                ReconciliationMode = ImportProductsRequest.Types.ReconciliationMode.Incremental,
                InputConfig = inputConfig,
                ErrorsConfig = errorsConfig
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var importRequestJson = JsonConvert.SerializeObject(importRequest, jsonSerializeSettings);

            Console.WriteLine("\nImport products from google cloud source. request: \n\n" + importRequestJson);
            return importRequest;
        }

        // Call the Retail API to import products
        [Attributes.Example]
        public static void ImportProductsFromGcs()
        {
            var importGcsRequest = GetImportProductsGcsRequest(gcsProductsObject);

            var importResponse = GetProductServiceClient().ImportProducts(importGcsRequest);

            Console.WriteLine("\nThe operation was started: \n" + importResponse.Name);
            Console.WriteLine("\nPlease wait till opeartion is done");

            var importResult = importResponse.PollUntilCompleted();

            Console.WriteLine("Import products operation is done\n");
            Console.WriteLine("Number of successfully imported products: " + importResult.Metadata.SuccessCount);
            Console.WriteLine("Number of failures during the importing: " + importResult.Metadata.FailureCount);
            Console.WriteLine("\nOperation result: \n" + importResult.Result);

            // The imported products needs to be indexed in the catalog before they become available for search.
            Console.WriteLine("Wait 2 - 5 minutes till products become indexed in the catalog, after that they will be available for search");
        }
    }
}
// [END retail_import_products_from_gcs]