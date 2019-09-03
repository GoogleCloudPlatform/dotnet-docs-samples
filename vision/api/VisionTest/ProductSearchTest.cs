using System;
using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class ProductSearchTest : IDisposable
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

        // For search tests. Product set must be indexed for search to succeed.
        private const string INDEXED_PRODUCT_SET = "indexed_product_set_id_for_testing";
        private const string INDEXED_PRODUCT_1 = "indexed_product_id_for_testing_1";

        private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        readonly CommandLineRunner _productSearch = new CommandLineRunner()
        {
            Main = ProductSearchProgram.Main,
            Command = "Product Search"
        };

        public ProductSearchTest()
        {
            // Create a indexed product set for TestProductSearch() and TestProductSearchGcs()
            // tests. These tests remain in the project after the test completes.
            var output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            if (!output.Stdout.Contains(INDEXED_PRODUCT_SET))
            {
                _productSearch.Run("create_product_set", _projectId, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_SET_DISPLAY_NAME);
            }

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            if (!output.Stdout.Contains(INDEXED_PRODUCT_1))
            {
                _productSearch.Run("create_product", _projectId, REGION_NAME, INDEXED_PRODUCT_1, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            }

            output = _productSearch.Run("list_ref_images", _projectId, REGION_NAME, INDEXED_PRODUCT_1);
            if (!output.Stdout.Contains(REF_IMAGE_ID))
            {
                _productSearch.Run("create_ref_image", _projectId, REGION_NAME, INDEXED_PRODUCT_1, REF_IMAGE_ID, REF_IMAGE_GCS_URI);
            }

            output = _productSearch.Run("list_products_in_set", _projectId, REGION_NAME, INDEXED_PRODUCT_SET);
            if (!output.Stdout.Contains(INDEXED_PRODUCT_1))
            {
                _productSearch.Run("add_product_to_set", _projectId, REGION_NAME, INDEXED_PRODUCT_1, INDEXED_PRODUCT_SET);
            }

            output = _productSearch.Run("get_product", _projectId, REGION_NAME, INDEXED_PRODUCT_1);
            if (!output.Stdout.Contains("style") || !output.Stdout.Contains("womens"))
            {
                _productSearch.Run("update_product_labels", _projectId, REGION_NAME, INDEXED_PRODUCT_1, "style,womens");
            }
        }

        [Fact]
        public void TestCreateProduct()
        {
            var output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);

            output = _productSearch.Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestGetProduct()
        {
            _productSearch.Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = _productSearch.Run("get_product", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(_projectId, output.Stdout);
        }

        [Fact]
        public void TestDeleteProduct()
        {
            _productSearch.Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);

            output = _productSearch.Run("delete_product", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Product deleted.", output.Stdout);

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestCreateProductSet()
        {
            var output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_ID, output.Stdout);

            output = _productSearch.Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_DISPLAY_NAME, output.Stdout);

            output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);
        }


        [Fact]
        public void TestGetProductSet()
        {
            _productSearch.Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = _productSearch.Run("get_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);
        }

        [Fact]
        public void TestDeleteProductSet()
        {
            _productSearch.Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);

            output = _productSearch.Run("delete_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_ID, output.Stdout);
        }

        [Fact]
        public void TestCreateReferenceImage()
        {
            _productSearch.Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = _productSearch.Run("list_ref_images", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);

            output = _productSearch.Run("create_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_ref_images", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);
        }


        [Fact]
        public void TestGetReferenceImage()
        {
            _productSearch.Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            _productSearch.Run("create_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);

            var output = _productSearch.Run("get_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);
        }

        [Fact]
        public void TestDeleteReferenceImage()
        {
            _productSearch.Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            _productSearch.Run("create_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);

            var output = _productSearch.Run("list_ref_images", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);

            output = _productSearch.Run("delete_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_ref_images", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);
        }

        [Fact]
        public void TestImportProductSets()
        {
            var output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_ID, output.Stdout);

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);
            Assert.DoesNotContain(PRODUCT_ID_2, output.Stdout);

            output = _productSearch.Run("import_product_set", _projectId, REGION_NAME, CSV_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);
            Assert.Contains(PRODUCT_ID_2, output.Stdout);
        }

        [Fact]
        public void TestAddProductToSet()
        {
            _productSearch.Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            _productSearch.Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = _productSearch.Run("list_products_in_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);

            output = _productSearch.Run("add_product_to_set", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products_in_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Contains(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestRemoveProductFromSet()
        {
            _productSearch.Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            _productSearch.Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            _productSearch.Run("add_product_to_set", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);

            var output = _productSearch.Run("list_products_in_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Contains(PRODUCT_ID, output.Stdout);

            output = _productSearch.Run("remove_product_from_set", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products_in_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestProductSearch()
        {
            var output = _productSearch.Run("get_similar_products", _projectId, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_CATEGORY, Path.Combine("data", IMAGE_URI_1), SEARCH_FILTER);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(INDEXED_PRODUCT_1, output.Stdout);
        }

        [Fact]
        public void TestProductSearchGcs()
        {
            var output = _productSearch.Run("get_similar_products_gcs", _projectId, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_CATEGORY, REF_IMAGE_GCS_URI, SEARCH_FILTER);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(INDEXED_PRODUCT_1, output.Stdout);
        }

        [Fact]
        public void TestPurgeProducsInProductSet()
        {
            var output = _productSearch.Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Assert.Equal(0, output.ExitCode);
            _productSearch.Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            _productSearch.Run("add_product_to_set", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);

            _productSearch.Run("purge_products_in_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID);

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);

        }

        [Fact]
        public void TestPurgeOrphanProducts()
        {
            _productSearch.Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            var output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);

            _productSearch.Run("purge_orphan_products", _projectId, REGION_NAME);

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);
        }

        public void Dispose()
        {
            var listRefImageOutput = _productSearch.Run("list_ref_images", _projectId, REGION_NAME, PRODUCT_ID);
            if (listRefImageOutput.Stdout.Contains(REF_IMAGE_ID))
            {
                _productSearch.Run("delete_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID);
            }

            var listProductsOutput = _productSearch.Run("list_products", _projectId, REGION_NAME);
            if (listProductsOutput.Stdout.Contains(PRODUCT_ID))
            {
                _productSearch.Run("delete_product", _projectId, REGION_NAME, PRODUCT_ID);
            }

            if (listProductsOutput.Stdout.Contains(PRODUCT_ID_2))
            {
                _productSearch.Run("delete_product", _projectId, REGION_NAME, PRODUCT_ID_2);
            }

            var listProductSetsOutput = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            if (listProductSetsOutput.Stdout.Contains(PRODUCT_SET_ID))
            {
                _productSearch.Run("delete_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            }
        }
    }
}
