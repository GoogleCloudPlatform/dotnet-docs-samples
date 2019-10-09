using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace GoogleCloudSamples
{
    public class ProductSearchTest : IDisposable
    {
        public readonly string REGION_NAME = "us-west1";
        public readonly string IMPORT_PRODUCT_ID = "fake_product_id_for_testing_1";
        public readonly string PRODUCT_ID = "fake_product_id_for_testing_" + TestUtil.RandomName();
        public readonly string PRODUCT_DISPLAY_NAME = "fake_product_display_name_for_testing";
        public readonly string PRODUCT_CATEGORY = "apparel";
        public readonly string PRODUCT_ID_2 = "fake_product_id_for_testing_2";
        public readonly string IMPORT_PRODUCT_ID_2 = "fake_product_id_for_testing";
        public readonly string PRODUCT_SET_ID = "fake_product_set_id_for_testing_" + TestUtil.RandomName();
        public readonly string IMPORT_PRODUCT_SET_ID = "fake_product_set_id_for_testing";
        public readonly string PRODUCT_SET_DISPLAY_NAME = "fake_product_set_display_name_for_testing";
        public readonly string REF_IMAGE_ID = "fake_ref_image_id_" + TestUtil.RandomName();
        public readonly string REF_IMAGE_GCS_URI = "gs://cloud-samples-data/vision/product_search/shoes_1.jpg";
        public readonly string CSV_GCS_URI = "gs://cloud-samples-data/vision/product_search/product_sets.csv";
        public readonly string IMAGE_URI_1 = "shoes_1.jpg";
        public readonly string IMAGE_URI_2 = "shoes_2.jpg";
        public readonly string SEARCH_FILTER = "style=womens";

        // For search tests. Product set must be indexed for search to succeed.
        public readonly string INDEXED_PRODUCT_SET = "indexed_product_set_id_for_testing_" + TestUtil.RandomName();
        public readonly string INDEXED_PRODUCT_1 = "indexed_product_id_for_testing_" + TestUtil.RandomName();

        public readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        readonly CommandLineRunner _productSearch = new CommandLineRunner()
        {
            Main = ProductSearchProgram.Main,
            Command = "Product Search"
        };

        // Keep a list of all the things created while running tests.
        public List<string[]> _createCommands = new List<string[]>();

        public ConsoleOutput Run(params string[] arguments) 
        {
            if (arguments[0].StartsWith("create_")) {
                _createCommands.Add(arguments);
            }
            return _productSearch.Run(arguments);
        }

        protected void CreateProductSet()
        {
            // Create a indexed product set for TestProductSearch() and TestProductSearchGcs()
            // tests. These tests remain in the project after the test completes.
            var output = Run("list_product_sets", _projectId, REGION_NAME);
            if (!output.Stdout.Contains(INDEXED_PRODUCT_SET))
            {
                Run("create_product_set", _projectId, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_SET_DISPLAY_NAME);
            }

            output = Run("list_products", _projectId, REGION_NAME);
            if (!output.Stdout.Contains(INDEXED_PRODUCT_1))
            {
                Run("create_product", _projectId, REGION_NAME, INDEXED_PRODUCT_1, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            }

            output = Run("list_ref_images", _projectId, REGION_NAME, INDEXED_PRODUCT_1);
            if (!output.Stdout.Contains(REF_IMAGE_ID))
            {
                Run("create_ref_image", _projectId, REGION_NAME, INDEXED_PRODUCT_1, REF_IMAGE_ID, REF_IMAGE_GCS_URI);
            }

            output = Run("list_products_in_set", _projectId, REGION_NAME, INDEXED_PRODUCT_SET);
            if (!output.Stdout.Contains(INDEXED_PRODUCT_1))
            {
                Run("add_product_to_set", _projectId, REGION_NAME, INDEXED_PRODUCT_1, INDEXED_PRODUCT_SET);
            }

            output = Run("get_product", _projectId, REGION_NAME, INDEXED_PRODUCT_1);
            if (!output.Stdout.Contains("style") || !output.Stdout.Contains("womens"))
            {
                Run("update_product_labels", _projectId, REGION_NAME, INDEXED_PRODUCT_1, "style,womens");
            }
        }

        public void Dispose()
        {
            // Clean up everything the test created.
            _createCommands.Reverse();
            var exceptions = new List<Exception>();
            foreach (string[] arguments in _createCommands)
            {
                var deleteCommand = arguments.ToList();
                deleteCommand[0] = deleteCommand[0].Replace("create_", "delete_");
                try
                {
                    Run(deleteCommand.ToArray());
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        [Fact]
        public void TestCreateProduct()
        {
            var output = Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);

            output = Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestGetProduct()
        {
            Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = Run("get_product", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(_projectId, output.Stdout);
        }

        [Fact]
        public void TestDeleteProduct()
        {
            Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);

            output = Run("delete_product", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Product deleted.", output.Stdout);

            output = Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestCreateProductSet()
        {
            var output = Run("list_product_sets", _projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_ID, output.Stdout);

            output = Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_DISPLAY_NAME, output.Stdout);

            output = Run("list_product_sets", _projectId, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);
        }


        [Fact]
        public void TestGetProductSet()
        {
            Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = Run("get_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);
        }

        [Fact]
        public void TestDeleteProductSet()
        {
            Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = Run("list_product_sets", _projectId, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);

            output = Run("delete_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_product_sets", _projectId, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_ID, output.Stdout);
        }

        [Fact]
        public void TestCreateReferenceImage()
        {
            Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = Run("list_ref_images", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);

            output = Run("create_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_ref_images", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);
        }


        [Fact]
        public void TestGetReferenceImage()
        {
            Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Run("create_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);

            var output = Run("get_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);
        }

        [Fact]
        public void TestDeleteReferenceImage()
        {
            Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Run("create_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);

            var output = Run("list_ref_images", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);

            output = Run("delete_ref_image", _projectId, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_ref_images", _projectId, REGION_NAME, PRODUCT_ID);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);
        }

        [Fact]
        public void TestImportProductSets()
        {
            var output = Run("import_product_set", _projectId, REGION_NAME, CSV_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_product_sets", _projectId, REGION_NAME);
            Assert.Contains(IMPORT_PRODUCT_SET_ID, output.Stdout);

            output = Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(IMPORT_PRODUCT_ID, output.Stdout);
            Assert.Contains(IMPORT_PRODUCT_ID_2, output.Stdout);
        }

        [Fact]
        public void TestAddProductToSet()
        {
            Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = Run("list_products_in_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);

            output = Run("add_product_to_set", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_products_in_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Contains(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestRemoveProductFromSet()
        {
            Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            Run("add_product_to_set", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);

            var output = Run("list_products_in_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.Contains(PRODUCT_ID, output.Stdout);

            output = Run("remove_product_from_set", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_products_in_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestProductSearch()
        {
            CreateProductSet();
            var output = Run("get_similar_products", _projectId, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_CATEGORY, Path.Combine("data", IMAGE_URI_1), SEARCH_FILTER);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(INDEXED_PRODUCT_1, output.Stdout);
        }

        [Fact]
        public void TestProductSearchGcs()
        {
            CreateProductSet();
            var output = Run("get_similar_products_gcs", _projectId, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_CATEGORY, REF_IMAGE_GCS_URI, SEARCH_FILTER);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(INDEXED_PRODUCT_1, output.Stdout);
        }

        [Fact]
        public void TestPurgeProductsInProductSet()
        {
            var output = Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Assert.Equal(0, output.ExitCode);

            Run("create_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            Run("add_product_to_set", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);

            output = Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);

            Run("purge_products_in_product_set", _projectId, REGION_NAME, PRODUCT_SET_ID);
            output = Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);
        }

        [Fact]
        public void TestPurgeOrphanProducts()
        {
            Run("create_product", _projectId, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            var output = Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);

            Run("purge_orphan_products", _projectId, REGION_NAME);

            output = Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);
        }
    }
}
