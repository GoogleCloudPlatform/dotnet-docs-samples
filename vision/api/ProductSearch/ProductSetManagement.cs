// Copyright(c) 2017 Google Inc.
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
using Google.Cloud.Vision.V1;
using System;

namespace GoogleCloudSamples
{
    class ProductSetsOptions {
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }

        [Value(1, HelpText = "Region name")]
        public string ComputeRegion { get; set; }
    }

    [Verb("create", HelpText = "Build a product set")]
    class CreateProductSetsOptions : ProductSetsOptions
    {
        [Value(2, HelpText = "Product Set ID")]
        public string ProductSetId { get; set; }

        [Value(3, HelpText = "Product Set Display Name")]
        public string ProductSetDisplayName { get; set; }
    }

    [Verb("list", HelpText = "List all product sets")]
    class ListProductSetsOptions : ProductSetsOptions
    { }

    [Verb("get", HelpText = "Get a product set")]
    class GetProductSetOptions : ProductSetsOptions
    { 
        [Value(2, HelpText = "Product Set ID")]
        public string ProductSetId { get; set; }
    }

    [Verb("delete", HelpText = "Delete a product set")]
    class DeleteProductSetOptions : ProductSetsOptions
    { 
        [Value(2, HelpText = "Product Set ID")]
        public string ProductSetId { get; set; }
    }

    public class ProductSetManagement
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((CreateProductSetsOptions opts) => CreateProductSet(opts))
                .Add((ListProductSetsOptions opts) => ListProductsSet(opts))
                .Add((GetProductSetOptions opts) => GetProductSet(opts))
                .Add((DeleteProductSetOptions opts) => DeleteProductSet(opts));
        }

        // [START vision_product_search_create_product_set]
        private static object CreateProductSet(CreateProductSetsOptions opts)
        {
            /*
             * dotnet run create vision-erschmid us-west1 someID someDisplayName
            */
            var client = ProductSearchClient.Create();

            var productSet = new ProductSet { 
                DisplayName = opts.ProductSetDisplayName
            };
            // Create a product set with the product set specification in the region.
            var request = new CreateProductSetRequest {
                // A resource that represents Google Cloud Platform location
                ParentAsLocationName = new LocationName(opts.ProjectID, 
                                                        opts.ComputeRegion),
                ProductSetId = opts.ProductSetId,
                ProductSet = productSet
            };

            // The response is the product set with the `name` populated
            var response = client.CreateProductSet(request);

            Console.WriteLine($"Product set name: {response.DisplayName}");

            return 0;
        }
        // [END vision_product_search_create_product_set]

        // [START vision_product_search_list_product_sets]
        private static object ListProductsSet(ListProductSetsOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new ListProductSetsRequest {
                // A resource that represents Google Cloud Platform location
                ParentAsLocationName = new LocationName(opts.ProjectID, opts.ComputeRegion)
            };

            // List all the product sets available in the region.
            var response = client.ListProductSets(request);

            foreach (var productSet in response)
            {
                var id = productSet.Name.Split("/")[5]; // Maybe store split result as array and then get length?
                Console.WriteLine($"Product set name: {productSet.DisplayName}");
                Console.WriteLine($"Product set ID: {id}");
                Console.WriteLine($"Product set index time:");
                Console.WriteLine($"\tseconds: {productSet.IndexTime.Seconds}");
                Console.WriteLine($"\tnanos: {productSet.IndexTime.Nanos}");
            }

            return 0;
        }
        // [END vision_product_search_list_product_sets]

        // [START vision_product_search_get_product_set]
        private static object GetProductSet(GetProductSetOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new GetProductSetRequest
            {
                // Get the full path of the product set.
                ProductSetName = new ProductSetName(opts.ProjectID, 
                                                    opts.ComputeRegion, 
                                                    opts.ProductSetId)
            };

            // Get the complete detail of the product set.
            var productSet = client.GetProductSet(request);

            var id = productSet.Name.Split("/")[5]; // Maybe store split result as array and then get length?
            Console.WriteLine($"Product set name: {productSet.DisplayName}");
            Console.WriteLine($"Product set ID: {id}");
            Console.WriteLine($"Product set index time:");
            Console.WriteLine($"\tseconds: {productSet.IndexTime.Seconds}");
            Console.WriteLine($"\tnanos: {productSet.IndexTime.Nanos}");

            return 0;
        }
        // [END vision_product_search_get_product_set]

        // [START vision_product_search_delete_product_set]
        private static object DeleteProductSet(DeleteProductSetOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new DeleteProductSetRequest {
                // Get the full path of the product set.
                ProductSetName = new ProductSetName(opts.ProjectID,
                                                    opts.ComputeRegion,
                                                    opts.ProductSetId)
            };

            // Delete the product set.
            client.DeleteProductSet(request);

            Console.WriteLine("Product set deleted.");
            return 0;
        }
        // [END vision_product_search_delete_product_set]
    }
}
