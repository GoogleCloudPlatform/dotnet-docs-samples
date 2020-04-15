using System;
using Google.Cloud.Vision.V1;
using CommandLine;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Google.Api.Gax.ResourceNames;

namespace GoogleCloudSamples
{
    [Verb("add_product_to_set", HelpText = "Add product to product set")]
    class AddProductToProductSetOptions : ProductWithIDOptions
    {
        [Value(3, HelpText = "Product Set ID")]
        public string ProductSetId { get; set; }
    }

    [Verb("list_products_in_set", HelpText = "List all products in product set")]
    class ListProductsInProductSetOptions : ProductSetWithIDOptions
    { }

    [Verb("remove_product_from_set", HelpText = "Remove product from product set")]
    class RemoveProductFromProductSetOptions : ProductWithIDOptions
    {
        [Value(3, HelpText = "Product Set ID")]
        public string ProductSetId { get; set; }
    }

    [Verb("purge_products_in_product_set", HelpText = "Delete all products in a product set")]
    class PurgeProductsInProductSetOptions : ProductSetWithIDOptions
    {
    }
    public class ProductInProductSetManagement
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((AddProductToProductSetOptions opts) => AddProductToProductSet(opts))
                .Add((ListProductsInProductSetOptions opts) => ListProductsInProductSet(opts))
                .Add((RemoveProductFromProductSetOptions opts) => RemoveProductFromProductSet(opts))
                .Add((PurgeProductsInProductSetOptions opts) => PurgeProductsInProductSet(opts));
        }

        // [START vision_product_search_add_product_to_product_set]
        private static int AddProductToProductSet(AddProductToProductSetOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new AddProductToProductSetRequest
            {
                // Get the full path of the products
                ProductAsProductName = new ProductName(opts.ProjectID,
                                                      opts.ComputeRegion,
                                                      opts.ProductID),
                // Get the full path of the product set.
                ProductSetName = new ProductSetName(opts.ProjectID,
                                                   opts.ComputeRegion,
                                                   opts.ProductSetId),
            };

            client.AddProductToProductSet(request);

            Console.WriteLine("Product added to product set.");

            return 0;
        }
        // [END vision_product_search_add_product_to_product_set]

        // [START vision_product_search_list_products_in_product_set]
        private static int ListProductsInProductSet(ListProductsInProductSetOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new ListProductsInProductSetRequest
            {
                // Get the full path of the product set.
                ProductSetName = new ProductSetName(opts.ProjectID,
                                                   opts.ComputeRegion,
                                                   opts.ProductSetId)
            };

            var products = client.ListProductsInProductSet(request);
            Console.WriteLine("Products in product set:");
            foreach (var product in products)
            {
                Console.WriteLine($"Product name: {product.Name}");
                Console.WriteLine($"Product id: {product.Name.Split("/").Last()}");
                Console.WriteLine($"Product display name: {product.DisplayName}");
                Console.WriteLine($"Product description: {product.Description}");
                Console.WriteLine($"Product category: {product.ProductCategory}");
                Console.WriteLine($"Product labels:");
                foreach (var label in product.ProductLabels)
                {
                    Console.WriteLine($"Label: {label}");
                }
            }

            return 0;
        }
        // [END vision_product_search_list_products_in_product_set]

        // [START vision_product_search_remove_product_from_product_set]
        private static int RemoveProductFromProductSet(RemoveProductFromProductSetOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new RemoveProductFromProductSetRequest
            {
                // Get the full path of the product.
                ProductAsProductName = new ProductName(opts.ProjectID,
                                                      opts.ComputeRegion,
                                                      opts.ProductID),
                // Get the full path of the product set.
                ProductSetName = new ProductSetName(opts.ProjectID,
                                                   opts.ComputeRegion,
                                                    opts.ProductSetId)
            };
            client.RemoveProductFromProductSet(request);
            Console.WriteLine("Product removed from product set.");
            return 0;
        }
        // [END vision_product_search_remove_product_from_product_set]

        // [START vision_product_search_purge_products_in_product_set]
        private static int PurgeProductsInProductSet(PurgeProductsInProductSetOptions opts)
        {
            var client = ProductSearchClient.Create();
            var parent = LocationName.Format(opts.ProjectID, opts.ComputeRegion);
            var productSetPurgeConfig = new ProductSetPurgeConfig
            {
                ProductSetId = opts.ProductSetId
            };
            var req = new PurgeProductsRequest
            {
                Parent = parent,
                ProductSetPurgeConfig = productSetPurgeConfig,
                Force = true
            };

            var response = client.PurgeProductsAsync(req);

            // wait until it finishes   
            response.Result.PollUntilCompleted();

            Console.WriteLine("Products removed from product set.");
            return 0;
        }
        // [END vision_product_search_purge_products_in_product_set]
    }
}
