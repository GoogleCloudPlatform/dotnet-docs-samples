using CommandLine;
using Google.Cloud.Vision.V1;
using System;
using System.Linq;

namespace GoogleCloudSamples
{
    class ReferenceImageOptions : ProductWithIDOptions
    {
        [Value(3, HelpText = "Reference image ID")]
        public string ReferenceImageID { get; set; }
    }

    [Verb("create_ref_image", HelpText = "Create a reference image for a product")]
    class CreateReferenceImageOptions : ReferenceImageOptions
    {
        [Value(4, HelpText = "Reference image Storage URI")]
        public string ReferenceImageURI { get; set; }
    }

    [Verb("list_ref_images", HelpText = "List reference images for a product")]
    class ListReferenceImagesOptions : ProductWithIDOptions
    { }

    [Verb("get_ref_image", HelpText = "Get reference image for product")]
    class GetReferenceImageOptions : ReferenceImageOptions
    { }

    [Verb("delete_ref_image", HelpText = "Delete a reference image for product")]
    class DeleteReferenceImageOptions : ReferenceImageOptions
    { }

    public class ReferenceImageManagement
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((CreateReferenceImageOptions opts) => CreateReferenceImage(opts))
                .Add((ListReferenceImagesOptions opts) => ListReferenceImagesOfProduct(opts))
                .Add((GetReferenceImageOptions opts) => GetReferenceImage(opts))
                .Add((DeleteReferenceImageOptions opts) => DeleteReferenceImage(opts));
        }

        // [START vision_product_search_create_reference_image]
        private static int CreateReferenceImage(CreateReferenceImageOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new CreateReferenceImageRequest
            {
                // Get the full path of the product.
                ParentAsProductName = new ProductName(opts.ProjectID,
                                                      opts.ComputeRegion,
                                                      opts.ProductID),
                ReferenceImageId = opts.ReferenceImageID,
                // Create a reference image.
                ReferenceImage = new ReferenceImage
                {
                    Uri = opts.ReferenceImageURI
                }
            };

            var referenceImage = client.CreateReferenceImage(request);

            Console.WriteLine($"Reference image name: {referenceImage.Name}");
            Console.WriteLine($"Reference image URI: {referenceImage.Uri}");

            return 0;
        }
        // [END vision_product_search_create_reference_image]

        // [START vision_product_search_list_reference_images]
        private static int ListReferenceImagesOfProduct(ListReferenceImagesOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new ListReferenceImagesRequest
            {
                // Get the full path of the product.
                ParentAsProductName = new ProductName(opts.ProjectID,
                                                      opts.ComputeRegion,
                                                      opts.ProductID)
            };

            var referenceImages = client.ListReferenceImages(request);
            foreach (var referenceImage in referenceImages)
            {
                var referenceImageID = referenceImage.Name.Split("/").Last();
                Console.WriteLine($"Reference image name: {referenceImage.Name}");
                Console.WriteLine($"Reference image id: {referenceImageID}");
                Console.WriteLine($"Reference image URI: {referenceImage.Uri}");
                Console.WriteLine("Reference image bounding polygons:");
                Console.WriteLine($"\t{referenceImage.BoundingPolys.ToString()}");
            }

            return 0;
        }
        // [END vision_product_search_list_reference_images]

        // [START vision_product_search_get_reference_image]
        private static int GetReferenceImage(GetReferenceImageOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new GetReferenceImageRequest
            {
                // Get the full path of the reference image.
                ReferenceImageName = new ReferenceImageName(opts.ProjectID,
                                                            opts.ComputeRegion,
                                                            opts.ProductID,
                                                            opts.ReferenceImageID)
            };

            var referenceImage = client.GetReferenceImage(request);

            var referenceImageID = referenceImage.Name.Split("/").Last();
            Console.WriteLine($"Reference image name: {referenceImage.Name}");
            Console.WriteLine($"Reference image id: {referenceImageID}");
            Console.WriteLine($"Reference image URI: {referenceImage.Uri}");
            Console.WriteLine("Reference image bounding polygons:");
            Console.WriteLine($"\t{referenceImage.BoundingPolys.ToString()}");
            return 0;
        }
        // [END vision_product_search_get_reference_image]

        // [START vision_product_search_delete_reference_image]
        private static int DeleteReferenceImage(DeleteReferenceImageOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new DeleteReferenceImageRequest
            {
                // Get the full path of the reference image.
                ReferenceImageName = new ReferenceImageName(opts.ProjectID,
                                                            opts.ComputeRegion,
                                                            opts.ProductID,
                                                            opts.ReferenceImageID)
            };

            client.DeleteReferenceImage(request);
            Console.WriteLine("Reference image deleted from product.");
            return 0;
        }

        // [END vision_product_search_delete_reference_image]
    }
}
