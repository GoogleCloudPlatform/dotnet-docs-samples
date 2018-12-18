using System;
using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class ProductSearchTest :  IDisposable
    {
        private const string REGION_NAME = "us-west1";
        private const string PRODUCT_ID = "fake_product_id_for_testing_1";
        private const string PRODUCT_DISPLAY_NAME = "fake_product_display_name_for_testing";
        private const string PRODUCT_CATEGORY = "apparel";
        private const string PRODUCT_ID_2 = "fake_product_id_for_testing_2";
        private const string PRODUCT_SET_ID = "fake_product_set_id_for_testing";
        private const string PRODUCT_SET_DISPLAY_NAME = "fake_product_set_display_name_for_testing";
        private const string REF_IMAGE_ID = "fake_ref_image_id";
        private const string REF_IMAGE_GCS_URI = "gs://cloud-samples-data/vision/product_search/shoes_1.jpg";
        private const string CSV_GCS_URI = "gs://cloud-samples-data/vision/product_search/product_sets.csv";
        private const string IMAGE_URI_1 = "shoes_1.jpg";
        private const string IMAGE_URI_2 = "shoes_2.jpg";
        private const string SEARCH_FILTER = "style=womens";

        private string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        readonly CommandLineRunner _productSearch = new CommandLineRunner()
        {
            Main = ProductSearchProgram.Main,
            Command = "Product Search"
        };


        [Fact]
        public void TestCreateProduct()
        {
            var output = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);

            output = _productSearch.Run("create_product", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestGetProduct()
        {
            _productSearch.Run("create_product", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = _productSearch.Run("get_product", projectId, REGION_NAME, PRODUCT_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(projectId, output.Stdout);
        }

        [Fact]
        public void TestDeleteProduct()
        {

            _productSearch.Run("create_product", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);

            output = _productSearch.Run("delete_product", projectId, REGION_NAME, PRODUCT_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Product deleted.", output.Stdout);

            output = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestCreateProductSet()
        {
            var output = _productSearch.Run("list_product_sets", projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_ID, output.Stdout);

            output = _productSearch.Run("create_product_set", projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_DISPLAY_NAME, output.Stdout);

            output = _productSearch.Run("list_product_sets", projectId, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);
        }


        [Fact]
        public void TestGetProductSet()
        {
            _productSearch.Run("create_product_set", projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = _productSearch.Run("get_product_set", projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);
        }

        [Fact]
        public void TestDeleteProductSet()
        {
            _productSearch.Run("create_product_set", projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = _productSearch.Run("list_product_sets", projectId, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);

            output = _productSearch.Run("delete_product_set", projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_product_sets", projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_ID, output.Stdout);
        }

        [Fact]
        public void TestCreateReferenceImage()
        {
            _productSearch.Run("create_product", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = _productSearch.Run("list_ref_images", projectId, REGION_NAME, PRODUCT_ID);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);

            output = _productSearch.Run("create_ref_image", projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_ref_images", projectId, REGION_NAME, PRODUCT_ID);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);
        }


        [Fact]
        public void TestGetReferenceImage()
        {
            _productSearch.Run("create_product", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            _productSearch.Run("create_ref_image", projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);

            var output = _productSearch.Run("get_ref_image", projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);
        }

        [Fact]
        public void TestDeleteReferenceImage()
        {
            _productSearch.Run("create_product", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            _productSearch.Run("create_ref_image", projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);

            var output = _productSearch.Run("list_ref_images", projectId, REGION_NAME, PRODUCT_ID);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);

            output = _productSearch.Run("delete_ref_image", projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_ref_images", projectId, REGION_NAME, PRODUCT_ID);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);

        }

        [Fact]
        public void TestImportProductSets()
        {
            var output = _productSearch.Run("list_product_sets", projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_ID, output.Stdout);

            output = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);
            Assert.DoesNotContain(PRODUCT_ID_2, output.Stdout);

            output = _productSearch.Run("import_product_set", projectId, REGION_NAME, CSV_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_product_sets", projectId, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);

            output = _productSearch.Run("list_products", projectId, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);
            Assert.Contains(PRODUCT_ID_2, output.Stdout);

        }

        [Fact]
        public void TestAddProductToSet()
        {
            _productSearch.Run("create_product", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            _productSearch.Run("create_product_set", projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = _productSearch.Run("list_products_in_set", projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);

            output = _productSearch.Run("add_product_to_set", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products_in_set", projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Contains(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestRemoveProductFromSet()
        {
            _productSearch.Run("create_product", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            _productSearch.Run("create_product_set", projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            _productSearch.Run("add_product_to_set", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);

            var output = _productSearch.Run("list_products_in_set", projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Contains(PRODUCT_ID, output.Stdout);

            output = _productSearch.Run("remove_product_from_set", projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products_in_set", projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestProductSearch()
        {
            _productSearch.Run("import_product_set", projectId, REGION_NAME, CSV_GCS_URI);

            var output = _productSearch.Run("get_similar_products", projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_CATEGORY, Path.Combine("data", IMAGE_URI_1), SEARCH_FILTER);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestProductSearchGcs()
        {
            _productSearch.Run("import_product_set", projectId, REGION_NAME, CSV_GCS_URI);

            var output = _productSearch.Run("get_similar_products_gcs", projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_CATEGORY, REF_IMAGE_GCS_URI, SEARCH_FILTER);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_ID, output.Stdout);
        }

        public void CleanUp()
        {
            var listRefImageOutput = _productSearch.Run("list_ref_images", projectId, REGION_NAME, PRODUCT_ID);
            if (listRefImageOutput.Stdout.Contains(REF_IMAGE_ID))
            {
                _productSearch.Run("delete_ref_image", projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID);
            }

            var listProductsOutput = _productSearch.Run("list_products", projectId, REGION_NAME);
            if (listProductsOutput.Stdout.Contains(PRODUCT_ID))
            {
                _productSearch.Run("delete_product", projectId, REGION_NAME, PRODUCT_ID);
            }

            if (listProductsOutput.Stdout.Contains(PRODUCT_ID_2))
            {
                _productSearch.Run("delete_product", projectId, REGION_NAME, PRODUCT_ID_2);
            }

            var listProductSetsOutput = _productSearch.Run("list_product_sets", projectId, REGION_NAME);
            if (listProductSetsOutput.Stdout.Contains(PRODUCT_SET_ID))
            {
                _productSearch.Run("delete_product_set", projectId, REGION_NAME, PRODUCT_SET_ID);
            }
        }


        public void Dispose()
        {
            CleanUp();
        }
    }
}
