using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class ProductSearchTest :  IDisposable
    {
        private const string REGION_NAME = "us-west1";
        private const string PRODUCT_NAME = "fake_product_id_for_testing_1";
        private const string PRODUCT_DISPLAY_NAME = "fake_product_display_name_for_testing";
        private const string PRODUCT_CATEGORY = "homegoods";
        private const string PRODUCT_NAME_2 = "fake_product_id_for_testing_2";
        private const string PRODUCT_SET_NAME = "fake_product_set_id_for_testing";
        private const string PRODUCT_SET_DISPLAY_NAME = "fake_product_set_display_name_for_testing";
        private const string REF_IMAGE_ID = "fake_ref_image_id";
        private const string REF_IMAGE_GCS_URI = "gs://cloud-samples-data/vision/product_search/shoes_1.jpg";
        private const string CSV_GCS_URI = "gs://cloud-samples-data/vision/product_search/product_sets.csv";
        private const string IMAGE_URI_1 = "shoes_1.jpg";
        private const string IMAGE_URI_2 = "shoes_2.jpg";

        private string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        readonly CommandLineRunner _productSearch = new CommandLineRunner()
        {
            Main = ProductSearchProgram.Main,
            Command = "Product Search"
        };

        /*
        [Fact]
        public void TestCreateProduct()
        {
            var output = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.DoesNotContain(output.Stdout, PRODUCT_NAME);

            output = _productSearch.Run("create_product", projectId, REGION_NAME, PRODUCT_NAME, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.Contains(output.Stdout, PRODUCT_NAME);
        }

        [Fact]
        public void TestGetProduct()
        {
            var output = _productSearch.Run("get_product", projectId, REGION_NAME, PRODUCT_NAME);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(projectId, output.Stdout);
        }

        [Fact]
        public void TestDeleteProduct()
        {
            var output = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.Contains(output.Stdout, PRODUCT_NAME);

            output = _productSearch.Run("delete_product", projectId, REGION_NAME, PRODUCT_NAME);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Product deleted.", output.Stdout);

            output = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.DoesNotContain(output.Stdout, PRODUCT_NAME);
        }
        */

        [Fact]
        public void TestCreateProductSet()
        {
            var output = _productSearch.Run("list_products_sets", projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_NAME, output.Stdout);

            output = _productSearch.Run("create_product_set", projectId, REGION_NAME, PRODUCT_SET_NAME, PRODUCT_SET_DISPLAY_NAME);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_DISPLAY_NAME, output.Stdout);

            output = _productSearch.Run("list_products_sets", projectId, REGION_NAME);
            Assert.Contains(PRODUCT_SET_NAME, output.Stdout);

            //CleanUp();
        }

        /*
        [Fact]
        public void TestGetProductSet()
        {
            var output = _productSearch.Run("get_product_set", projectId, REGION_NAME, PRODUCT_SET_NAME);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_NAME, output.Stdout);
        }

        [Fact]
        public void TestDeleteProductSet()
        {
            var output = _productSearch.Run("list_products_sets", projectId, REGION_NAME);
            Assert.Contains(PRODUCT_SET_NAME, output.Stdout);

            output = _productSearch.Run("delete_product_set", projectId, REGION_NAME, PRODUCT_SET_NAME);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products_sets", projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_NAME, output.Stdout);
        }*/

        /*
        [Fact]
        public void TestCreateReferenceImage()
        {
            _productSearch.Run("create_product", projectId, REGION_NAME, PRODUCT_NAME, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = _productSearch.Run("list_ref_images", projectId, REGION_NAME, PRODUCT_NAME);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);

            output = _productSearch.Run("create_ref_image", projectId, REGION_NAME, PRODUCT_NAME, REF_IMAGE_ID, REF_IMAGE_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_ref_images", projectId, REGION_NAME, PRODUCT_NAME);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);
        }

        [Fact]
        public void TestGetReferenceImage()
        {
            var output = _productSearch.Run("get_ref_image", projectId, REGION_NAME, PRODUCT_NAME, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);
        }

        [Fact]
        public void TestDeleteReferenceImage()
        {
            var output = _productSearch.Run("list_ref_images", projectId, REGION_NAME, PRODUCT_NAME);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);

            output = _productSearch.Run("delete_ref_image", projectId, REGION_NAME, PRODUCT_NAME, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_ref_images", projectId, REGION_NAME, PRODUCT_NAME);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);

            CleanUp();
        }

        [Fact]
        public void TestImportProductSets()
        {
            var productSetsBeforeOutput = _productSearch.Run("list_product_sets", projectId, REGION_NAME);
            Assert.DoesNotContain(productSetsBeforeOutput.Stdout, PRODUCT_SET_NAME);

            var productsBeforeOutput = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.DoesNotContain(productsBeforeOutput.Stdout, PRODUCT_NAME);
            Assert.DoesNotContain(productsBeforeOutput.Stdout, PRODUCT_NAME_2);

            var output = _productSearch.Run("import_product_set", projectId, REGION_NAME, CSV_GCS_URI);

            Assert.Equal(0, output.ExitCode);

            var productSetsAfterOutput = _productSearch.Run("list_product_sets", projectId, REGION_NAME);
            Assert.Contains(productSetsAfterOutput.Stdout, PRODUCT_SET_NAME);

            var productsAfterOutput = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.Contains(productsAfterOutput.Stdout, PRODUCT_NAME);
            Assert.Contains(productsAfterOutput.Stdout, PRODUCT_NAME_2);

            CleanUp();
        }
        */

        // ProductInProductSetManagement
        // - add_product_to_set PROJECT_ID REGION PRODUCT_ID PRODUCT_SET_ID
        // - list_products_in_set PROJECT_ID REGION PRODUCT_SET_ID
        // - remove_product_from_set PROJECT_ID REGION PRODUCT_ID PRODUCT_SET_ID

        // ProductSearch
        // - get_similar_products PROJECT_ID REGION PRODUCT_ID PRODUCT_CATEGORY FILEPATH FILTER
        // - get_similar_products_gcs PROJECT_ID REGION PRODUCT_ID PRODUCT CATEGORY GCS_URI FILTER

        public void SetUp()
        {
            
        }

        public void CleanUp()
        {
            var listRefImageOutput = _productSearch.Run("list_ref_images", projectId, REGION_NAME, PRODUCT_NAME);
            if (listRefImageOutput.Stdout.Contains(REF_IMAGE_ID))
            {
                _productSearch.Run("delete_ref_image", projectId, REGION_NAME, PRODUCT_NAME, REF_IMAGE_ID);
            }

            var listProductsOutput = _productSearch.Run("list_products", projectId, REGION_NAME);
            if (listProductsOutput.Stdout.Contains(PRODUCT_NAME))
            {
                _productSearch.Run("delete_product", projectId, REGION_NAME, PRODUCT_NAME);
            }

            if (listProductsOutput.Stdout.Contains(PRODUCT_NAME_2))
            {
                _productSearch.Run("delete_product", projectId, REGION_NAME, PRODUCT_NAME_2);
            }

            var listProductSetsOutput = _productSearch.Run("list_product_sets", projectId, REGION_NAME);
            if (listProductSetsOutput.Stdout.Contains(PRODUCT_SET_NAME))
            {
                _productSearch.Run("delete_product_set", projectId, REGION_NAME, PRODUCT_SET_NAME);
            }
        }


        public void Dispose()
        {
            CleanUp();
        }
    }
}
