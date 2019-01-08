using CommandLine;
// [START vision_product_search_tutorial_import]
using Google.Cloud.Vision.V1;
// [END vision_product_search_tutorial_import]
using System;

namespace GoogleCloudSamples
{
    [Verb("get_similar_products", HelpText = "Get similar products to image")]
    class GetSimilarProductsOptions : ProductSetWithIDOptions
    {
        [Value(3, HelpText = "Product category")]
        public string ProductCategory { get; set; }

        [Value(4, HelpText = "Local file path of the image to search for")]
        public string FilePath { get; set; }

        [Value(5, HelpText = "Condition to apply to the label")]
        public string Filter { get; set; }
    }

    [Verb("get_similar_products_gcs", HelpText = "Get similar products to image")]
    class GetSimilarProductsGcsOptions : ProductSetWithIDOptions
    {
        [Value(3, HelpText = "Product category")]
        public string ProductCategory { get; set; }

        [Value(4, HelpText = "Local file path of the image to search for")]
        public string GcsUri { get; set; }

        [Value(5, HelpText = "Condition to apply to the label")]
        public string Filter { get; set; }
    }

    public class ProductSearch
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((GetSimilarProductsOptions opts) => GetSimilarProductsFile(opts))
                .Add((GetSimilarProductsGcsOptions opts) => GetSimilarProductsGcs(opts));
        }

        // [START vision_product_search_get_similar_products]
        private static int GetSimilarProductsFile(GetSimilarProductsOptions opts)
        {
            // Create annotate image request along with product search feature.
            Image image = Image.FromFile(opts.FilePath);
            var imageAnnotatorClient = ImageAnnotatorClient.Create();

            // Product Search specific parameters
            var productSearchParams = new ProductSearchParams
            {
                ProductSetAsProductSetName = new ProductSetName(opts.ProjectID,
                                                                opts.ComputeRegion,
                                                                opts.ProductSetId),
                ProductCategories = { opts.ProductCategory },
                Filter = opts.Filter
            };

            // Search products similar to the image.
            var results = imageAnnotatorClient.DetectSimilarProducts(image, productSearchParams);

            Console.WriteLine("Similar products:");
            foreach (var result in results.Results)
            {
                var product = result.Product;
                Console.WriteLine($"Score (Confidence): {result.Score}");
                Console.WriteLine($"Image name: {result.Image}");
                Console.WriteLine($"Product name: {product.Name}");
                Console.WriteLine($"Product display name: {product.DisplayName}");
                Console.WriteLine($"Product description: {product.Description}");
                Console.WriteLine($"Product labels: {product.ProductLabels}");
            }
            return 0;
        }
        // [END vision_product_search_get_similar_products]

        // [START vision_product_search_get_similar_products_gcs]
        private static int GetSimilarProductsGcs(GetSimilarProductsGcsOptions opts)
        {
            // Create annotate image request along with product search feature.
            Image image = Image.FromUri(opts.GcsUri);
            var imageAnnotatorClient = ImageAnnotatorClient.Create();

            // Product Search specific parameters
            var productSearchParams = new ProductSearchParams
            {
                ProductSetAsProductSetName = new ProductSetName(opts.ProjectID,
                                                                opts.ComputeRegion,
                                                                opts.ProductSetId),
                ProductCategories = { opts.ProductCategory },
                Filter = opts.Filter
            };

            // Search products similar to the image.
            var results = imageAnnotatorClient.DetectSimilarProducts(image, productSearchParams);

            Console.WriteLine("Similar products:");
            foreach (var result in results.Results)
            {
                var product = result.Product;
                Console.WriteLine($"Score (Confidence): {result.Score}");
                Console.WriteLine($"Image name: {result.Image}");
                Console.WriteLine($"Product name: {product.Name}");
                Console.WriteLine($"Product display name: {product.DisplayName}");
                Console.WriteLine($"Product description: {product.Description}");
                Console.WriteLine($"Product labels: {product.ProductLabels}");
            }
            return 0;
        }
        // [END vision_product_search_get_similar_products_gcs]
    }
}
