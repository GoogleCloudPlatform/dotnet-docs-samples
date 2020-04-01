using CommandLine;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Vision.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Linq;

namespace GoogleCloudSamples
{
    class ProductWithIDOptions : BaseOptions
    {
        [Value(2, HelpText = "Product's ID")]
        public string ProductID { get; set; }
    }

    [Verb("create_product", HelpText = "Build a product set")]
    class CreateProductOptions : ProductWithIDOptions
    {
        [Value(3, HelpText = "Product's display name")]
        public string DisplayName { get; set; }

        [Value(4, HelpText = "Product's category")]
        public string ProductCategory { get; set; }
    }

    [Verb("list_products", HelpText = "List all products in project")]
    class ListProductsOptions : BaseOptions
    { }

    [Verb("get_product", HelpText = "Get a product")]
    class GetProductOptions : ProductWithIDOptions
    { }

    [Verb("update_product_labels", HelpText = "Update a product's labels")]
    class UpdateProductLabelsOptions : ProductWithIDOptions
    {
        [Value(3, HelpText = "Product label")]
        public string Labels { get; set; }
    }

    [Verb("delete_product", HelpText = "Delete a product")]
    class DeleteProductOptions : ProductWithIDOptions
    { }

    [Verb("purge_orphan_products", HelpText = "Perform the purge")]
    class PurgeOrphanProductsOptions : BaseOptions
    {
    }

    public class ProductManagement
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((CreateProductOptions opts) => CreateProduct(opts))
                .Add((ListProductsOptions opts) => ListProducts(opts))
                .Add((GetProductOptions opts) => GetProduct(opts))
                .Add((UpdateProductLabelsOptions opts) => UpdateProductLabels(opts))
                .Add((DeleteProductOptions opts) => DeleteProduct(opts))
                .Add((PurgeOrphanProductsOptions opts) => PurgeOrphanProducts(opts));
        }

        // [START vision_product_search_create_product]
        private static int CreateProduct(CreateProductOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new CreateProductRequest
            {
                // A resource that represents Google Cloud Platform location.
                ParentAsLocationName = new LocationName(opts.ProjectID,
                                                        opts.ComputeRegion),
                // Set product category and product display name
                Product = new Product
                {
                    DisplayName = opts.DisplayName,
                    ProductCategory = opts.ProductCategory
                },
                ProductId = opts.ProductID
            };

            // The response is the product with the `name` field populated.
            var product = client.CreateProduct(request);

            Console.WriteLine($"Product name: {product.Name}");
            return 0;
        }
        // [END vision_product_search_create_product]

        // [START vision_product_search_list_products]
        private static int ListProducts(ListProductsOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new ListProductsRequest
            {
                // A resource that represents Google Cloud Platform location.
                ParentAsLocationName = new LocationName(opts.ProjectID,
                                                        opts.ComputeRegion)
            };

            var products = client.ListProducts(request);
            foreach (var product in products)
            {
                var productId = product.Name.Split("/").Last();
                Console.WriteLine($"\nProduct name: {product.Name}");
                Console.WriteLine($"Product id: {productId}");
                Console.WriteLine($"Product display name: {product.DisplayName}");
                Console.WriteLine($"Product category: {product.ProductCategory}");
                Console.WriteLine($"Product labels:");
                foreach (var label in product.ProductLabels)
                {
                    Console.WriteLine($"\tLabel: {label.ToString()}");
                }
            }
            return 0;
        }
        // [END vision_product_search_list_products]

        // [START vision_product_search_get_product]
        private static int GetProduct(GetProductOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new GetProductRequest
            {
                // Get the full path of the product.
                ProductName = new ProductName(opts.ProjectID,
                                              opts.ComputeRegion,
                                              opts.ProductID)
            };

            var product = client.GetProduct(request);

            var productId = product.Name.Split("/").Last();
            Console.WriteLine($"\nProduct name: {product.Name}");
            Console.WriteLine($"Product id: {productId}");
            Console.WriteLine($"Product display name: {product.DisplayName}");
            Console.WriteLine($"Product category: {product.ProductCategory}");
            Console.WriteLine($"Product labels:");
            foreach (var label in product.ProductLabels)
            {
                Console.WriteLine($"\tLabel: {label.ToString()}");
            }
            return 0;
        }
        // [END vision_product_search_get_product]

        // [START vision_product_search_update_product_labels]
        private static int UpdateProductLabels(UpdateProductLabelsOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new UpdateProductRequest
            {
                Product = new Product
                {
                    // Get the name of the product
                    ProductName = new ProductName(opts.ProjectID,
                                                 opts.ComputeRegion,
                                                 opts.ProductID),
                    // Set the product name, product label, and product display
                    // name. Multiple labels are also supported.
                    ProductLabels =
                    {
                        new Product.Types.KeyValue
                        {
                            Key = opts.Labels.Split(",").First(),
                            Value = opts.Labels.Split(",").Last()
                        }
                    }
                },
                // Updating only the product_labels field here.
                UpdateMask = new FieldMask
                {
                    Paths = { "product_labels" }
                }
            };

            // This overwrites the product_labels.
            var product = client.UpdateProduct(request);

            var productId = product.Name.Split("/").Last();
            Console.WriteLine($"\nProduct name: {product.Name}");
            Console.WriteLine($"Product id: {productId}");
            Console.WriteLine($"Product display name: {product.DisplayName}");
            Console.WriteLine($"Product category: {product.ProductCategory}");
            Console.WriteLine($"Product labels:");
            foreach (var label in product.ProductLabels)
            {
                Console.WriteLine($"\tLabel: {label.ToString()}");
            }
            return 0;
        }
        // [END vision_product_search_update_product_labels]

        // [START vision_product_search_delete_product]
        private static int DeleteProduct(DeleteProductOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new DeleteProductRequest
            {
                // Get the full path of the product.
                ProductName = new ProductName(opts.ProjectID,
                                             opts.ComputeRegion,
                                             opts.ProductID)
            };

            client.DeleteProduct(request);
            Console.WriteLine("Product deleted.");

            return 0;
        }
        // [END vision_product_search_delete_product]

        // [START vision_product_search_purge_orphan_products]
        private static int PurgeOrphanProducts(PurgeOrphanProductsOptions opts)
        {
            var client = ProductSearchClient.Create();
            var parent = LocationName.Format(opts.ProjectID, opts.ComputeRegion);

            var request = new PurgeProductsRequest
            {
                Parent = parent,
                DeleteOrphanProducts = true,
                Force = true
            };

            // Purge operation is async.
            var operation = client.PurgeProductsAsync(request);

            // wait until long operation to finish.
            operation.Result.PollUntilCompleted();
            Console.WriteLine("Orphan products deleted.");

            return 0;
        }
        // [END vision_product_search_purge_orphan_products]
    }
}
