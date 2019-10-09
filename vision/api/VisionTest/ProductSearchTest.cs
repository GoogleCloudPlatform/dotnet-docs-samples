using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace GoogleCloudSamples
{
    public class ProductSearchTestsBase
    {
        public string REGION_NAME { get; private set; } = "us-west1";
        public string PRODUCT_ID { get; private set; } = "fake_product_id_for_testing_1";
        public string PRODUCT_DISPLAY_NAME { get; private set; } = "fake_product_display_name_for_testing";
        public string PRODUCT_CATEGORY { get; private set; } = "apparel";
        public string PRODUCT_ID_2 { get; private set; } = "fake_product_id_for_testing_2";
        public string PRODUCT_SET_ID { get; private set; } = "fake_product_set_id_for_testing";
        public string PRODUCT_SET_DISPLAY_NAME { get; private set; } = "fake_product_set_display_name_for_testing";
        public string REF_IMAGE_ID { get; private set; } = "fake_ref_image_id";
        public string REF_IMAGE_GCS_URI { get; private set; } = "gs://cloud-samples-data/vision/product_search/shoes_1.jpg";
        public string CSV_GCS_URI { get; private set; } = "gs://cloud-samples-data/vision/product_search/product_sets.csv";
        public string IMAGE_URI_1 { get; private set; } = "shoes_1.jpg";
        public string IMAGE_URI_2 { get; private set; } = "shoes_2.jpg";
        public string SEARCH_FILTER { get; private set; } = "style=womens";

        // For search tests. Product set must be indexed for search to succeed.
        public string INDEXED_PRODUCT_SET { get; private set; } = "indexed_product_set_id_for_testing";
        public string INDEXED_PRODUCT_1 { get; private set; } = "indexed_product_id_for_testing_1";
        public readonly string PROJECT_ID = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        private readonly CommandLineRunner _productSearch = new CommandLineRunner()
        {
            Main = ProductSearchProgram.Main,
            Command = "Product Search"
        };

        // Keep a list of all the things created while running tests.
        private readonly List<string[]> _createCommands = new List<string[]>();

        /// <summary>
        ///  Run the command and track all cloud assets that were created.
        /// </summary>
        /// <param name="arguments">The command arguments.</param>
        public ConsoleOutput Run(params string[] arguments)
        {
            if (arguments[0].StartsWith("create_"))
            {
                _createCommands.Add(arguments);
            }
            return _productSearch.Run(arguments);
        }

        /// <summary>
        /// Delete all the things created in Run() commands.
        /// </summary>
        protected void DeleteCreations()
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

        /// <summary>
        /// Add a random chunk to all the Ids used in the tests, so that
        /// multiple machines can run the same tests at the same time
        /// in the same Google Cloud project without interfering with each
        /// other.
        /// </summary>
        protected void RandomizeIds()
        {
            PRODUCT_ID += TestUtil.RandomName();
            PRODUCT_ID_2 += TestUtil.RandomName();
            PRODUCT_SET_ID += TestUtil.RandomName();
            REF_IMAGE_ID += TestUtil.RandomName();
            INDEXED_PRODUCT_1 += TestUtil.RandomName();
            INDEXED_PRODUCT_SET += TestUtil.RandomName();
        }
    }

    public class ProductSearchTests : ProductSearchTestsBase, IDisposable
    {
        public ProductSearchTests()
        {
            RandomizeIds();
        }

        public void Dispose()
        {
            DeleteCreations();
        }

        [Fact]
        public void TestCreateProduct()
        {
            var output = Run("list_products", PROJECT_ID, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);

            output = Run("create_product", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_products", PROJECT_ID, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestGetProduct()
        {
            Run("create_product", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = Run("get_product", PROJECT_ID, REGION_NAME, PRODUCT_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PROJECT_ID, output.Stdout);
        }

        [Fact]
        public void TestDeleteProduct()
        {
            Run("create_product", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = Run("list_products", PROJECT_ID, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);

            output = Run("delete_product", PROJECT_ID, REGION_NAME, PRODUCT_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Product deleted.", output.Stdout);

            output = Run("list_products", PROJECT_ID, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestCreateProductSet()
        {
            var output = Run("list_product_sets", PROJECT_ID, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_ID, output.Stdout);

            output = Run("create_product_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_DISPLAY_NAME, output.Stdout);

            output = Run("list_product_sets", PROJECT_ID, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);
        }


        [Fact]
        public void TestGetProductSet()
        {
            Run("create_product_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = Run("get_product_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);
        }

        [Fact]
        public void TestDeleteProductSet()
        {
            Run("create_product_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = Run("list_product_sets", PROJECT_ID, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);

            output = Run("delete_product_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_product_sets", PROJECT_ID, REGION_NAME);
            Assert.DoesNotContain(PRODUCT_SET_ID, output.Stdout);
        }

        [Fact]
        public void TestCreateReferenceImage()
        {
            Run("create_product", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = Run("list_ref_images", PROJECT_ID, REGION_NAME, PRODUCT_ID);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);

            output = Run("create_ref_image", PROJECT_ID, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_ref_images", PROJECT_ID, REGION_NAME, PRODUCT_ID);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);
        }


        [Fact]
        public void TestGetReferenceImage()
        {
            Run("create_product", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Run("create_ref_image", PROJECT_ID, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);

            var output = Run("get_ref_image", PROJECT_ID, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);
        }

        [Fact]
        public void TestDeleteReferenceImage()
        {
            Run("create_product", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Run("create_ref_image", PROJECT_ID, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID, REF_IMAGE_GCS_URI);

            var output = Run("list_ref_images", PROJECT_ID, REGION_NAME, PRODUCT_ID);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);

            output = Run("delete_ref_image", PROJECT_ID, REGION_NAME, PRODUCT_ID, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_ref_images", PROJECT_ID, REGION_NAME, PRODUCT_ID);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);
        }

        [Fact]
        public void TestAddProductToSet()
        {
            Run("create_product", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Run("create_product_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);

            var output = Run("list_products_in_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);

            output = Run("add_product_to_set", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_products_in_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID);
            Assert.Contains(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestRemoveProductFromSet()
        {
            Run("create_product", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Run("create_product_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            Run("add_product_to_set", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);

            var output = Run("list_products_in_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID);
            Assert.Contains(PRODUCT_ID, output.Stdout);

            output = Run("remove_product_from_set", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_products_in_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID);
            Assert.DoesNotContain(PRODUCT_ID, output.Stdout);
        }

        [Fact]
        public void TestPurgeProductsInProductSet()
        {
            var output = Run("create_product", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Assert.Equal(0, output.ExitCode);

            Run("create_product_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID, PRODUCT_SET_DISPLAY_NAME);
            Run("add_product_to_set", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_SET_ID);

            output = Run("list_products", PROJECT_ID, REGION_NAME);
            Assert.Contains(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);

            Run("purge_products_in_product_set", PROJECT_ID, REGION_NAME, PRODUCT_SET_ID);
            output = Run("list_products", PROJECT_ID, REGION_NAME);
            Assert.DoesNotContain(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);
        }

        [Fact]
        public void TestPurgeOrphanProducts()
        {
            Run("create_product", PROJECT_ID, REGION_NAME, PRODUCT_ID, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            var output = Run("list_products", PROJECT_ID, REGION_NAME);
            Assert.Contains(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);

            Run("purge_orphan_products", PROJECT_ID, REGION_NAME);

            output = Run("list_products", PROJECT_ID, REGION_NAME);
            Assert.DoesNotContain(String.Format("Product id: {0}", PRODUCT_ID), output.Stdout);
        }
    }

    // These tests all require products and indexes that live longer than the
    // test.
    public class ProductSearchCodependentTests : ProductSearchTestsBase
    {
        protected void CreateProductSet()
        {
            // Create a indexed product set for TestProductSearch() and TestProductSearchGcs()
            // tests. These tests remain in the project after the test completes.
            var output = Run("list_product_sets", PROJECT_ID, REGION_NAME);
            if (!output.Stdout.Contains(INDEXED_PRODUCT_SET))
            {
                Run("create_product_set", PROJECT_ID, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_SET_DISPLAY_NAME);
            }

            output = Run("list_products", PROJECT_ID, REGION_NAME);
            if (!output.Stdout.Contains(INDEXED_PRODUCT_1))
            {
                Run("create_product", PROJECT_ID, REGION_NAME, INDEXED_PRODUCT_1, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            }

            output = Run("list_ref_images", PROJECT_ID, REGION_NAME, INDEXED_PRODUCT_1);
            if (!output.Stdout.Contains(REF_IMAGE_ID))
            {
                Run("create_ref_image", PROJECT_ID, REGION_NAME, INDEXED_PRODUCT_1, REF_IMAGE_ID, REF_IMAGE_GCS_URI);
            }

            output = Run("list_products_in_set", PROJECT_ID, REGION_NAME, INDEXED_PRODUCT_SET);
            if (!output.Stdout.Contains(INDEXED_PRODUCT_1))
            {
                Run("add_product_to_set", PROJECT_ID, REGION_NAME, INDEXED_PRODUCT_1, INDEXED_PRODUCT_SET);
            }

            output = Run("get_product", PROJECT_ID, REGION_NAME, INDEXED_PRODUCT_1);
            if (!output.Stdout.Contains("style") || !output.Stdout.Contains("womens"))
            {
                Run("update_product_labels", PROJECT_ID, REGION_NAME, INDEXED_PRODUCT_1, "style,womens");
            }
        }

        [Fact]
        public void TestProductSearch()
        {
            CreateProductSet();
            var output = Run("get_similar_products", PROJECT_ID, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_CATEGORY, Path.Combine("data", IMAGE_URI_1), SEARCH_FILTER);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(INDEXED_PRODUCT_1, output.Stdout);
        }

        [Fact]
        public void TestProductSearchGcs()
        {
            CreateProductSet();
            var output = Run("get_similar_products_gcs", PROJECT_ID, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_CATEGORY, REF_IMAGE_GCS_URI, SEARCH_FILTER);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(INDEXED_PRODUCT_1, output.Stdout);
        }

        [Fact]
        public void TestImportProductSets()
        {
            var output = Run("import_product_set", PROJECT_ID, REGION_NAME, CSV_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_product_sets", PROJECT_ID, REGION_NAME);
            Assert.Contains(PRODUCT_SET_ID, output.Stdout);

            output = Run("list_products", PROJECT_ID, REGION_NAME);
            Assert.Contains(PRODUCT_ID, output.Stdout);
            Assert.Contains(PRODUCT_ID_2, output.Stdout);
        }
    }
}
