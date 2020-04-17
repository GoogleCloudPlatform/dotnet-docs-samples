using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace GoogleCloudSamples
{
    public class ProductSearchTestsBase
    {
        protected string RegionName { get; private set; } = "us-west1";
        protected string ProductId { get; private set; } = "fake_product_id_for_testing_1" + TestUtil.RandomName();
        protected string ProductDisplayName { get; private set; } = "fake_product_display_name_for_testing";
        protected string ProductCategory { get; private set; } = "apparel";
        protected string ProductId2 { get; private set; } = "fake_product_id_for_testing_2" + TestUtil.RandomName();
        protected string ImportedProductSetId { get; private set; } = "fake_product_set_id_for_testing";
        protected string ImportedProductId { get; private set; } = "fake_product_id_for_testing_1";
        protected string ProductSetId { get; private set; } = "fake_product_set_id_for_testing" + TestUtil.RandomName();
        protected string ProductSetDisplayName { get; private set; } = "fake_product_set_display_name_for_testing";
        protected string RefImageId { get; private set; } = "fake_ref_image_id" + TestUtil.RandomName();
        protected string RefImageGcsUri { get; private set; } = "gs://cloud-samples-data/vision/product_search/shoes_1.jpg";
        protected string CsvGcsUri { get; private set; } = "gs://cloud-samples-data/vision/product_search/product_sets.csv";
        protected string ImageUri1 { get; private set; } = "shoes_1.jpg";
        protected string ImageUri2 { get; private set; } = "shoes_2.jpg";
        protected string SearchFilter { get; private set; } = "style=womens";

        // For search tests. Product set must be indexed for search to succeed.
        protected string IndexedProductSet { get; private set; } = "indexed_product_set_id_for_testing";
        protected string IndexedProduct1 { get; private set; } = "indexed_product_id_for_testing_1";
        protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

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
            List<string[]> commands = new List<string[]>(_createCommands);
            _createCommands.Clear();
            commands.Reverse();

            var exceptions = new List<Exception>();
            foreach (string[] command in commands)
            {
                command[0] = command[0].Replace("create_", "delete_");
                try
                {
                    Run(command);
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
            ProductId += TestUtil.RandomName();
            ProductId2 += TestUtil.RandomName();
            ProductSetId += TestUtil.RandomName();
            RefImageId += TestUtil.RandomName();
            IndexedProduct1 += TestUtil.RandomName();
            IndexedProductSet += TestUtil.RandomName();
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
            var output = Run("list_products", ProjectId, RegionName);
            Assert.DoesNotContain(ProductId, output.Stdout);

            output = Run("create_product", ProjectId, RegionName, ProductId, ProductDisplayName, ProductCategory);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_products", ProjectId, RegionName);
            Assert.Contains(ProductId, output.Stdout);
        }

        [Fact]
        public void TestGetProduct()
        {
            Run("create_product", ProjectId, RegionName, ProductId, ProductDisplayName, ProductCategory);

            var output = Run("get_product", ProjectId, RegionName, ProductId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(ProjectId, output.Stdout);
        }

        [Fact]
        public void TestDeleteProduct()
        {
            Run("create_product", ProjectId, RegionName, ProductId, ProductDisplayName, ProductCategory);

            var output = Run("list_products", ProjectId, RegionName);
            Assert.Contains(ProductId, output.Stdout);

            output = Run("delete_product", ProjectId, RegionName, ProductId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Product deleted.", output.Stdout);

            output = Run("list_products", ProjectId, RegionName);
            Assert.DoesNotContain(ProductId, output.Stdout);
        }

        [Fact]
        public void TestCreateProductSet()
        {
            var output = Run("list_product_sets", ProjectId, RegionName);
            Assert.DoesNotContain(ProductSetId, output.Stdout);

            output = Run("create_product_set", ProjectId, RegionName, ProductSetId, ProductSetDisplayName);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(ProductSetDisplayName, output.Stdout);

            output = Run("list_product_sets", ProjectId, RegionName);
            Assert.Contains(ProductSetId, output.Stdout);
        }


        [Fact]
        public void TestGetProductSet()
        {
            Run("create_product_set", ProjectId, RegionName, ProductSetId, ProductSetDisplayName);

            var output = Run("get_product_set", ProjectId, RegionName, ProductSetId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(ProductSetId, output.Stdout);
        }

        [Fact]
        public void TestDeleteProductSet()
        {
            Run("create_product_set", ProjectId, RegionName, ProductSetId, ProductSetDisplayName);

            var output = Run("list_product_sets", ProjectId, RegionName);
            Assert.Contains(ProductSetId, output.Stdout);

            output = Run("delete_product_set", ProjectId, RegionName, ProductSetId);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_product_sets", ProjectId, RegionName);
            Assert.DoesNotContain(ProductSetId, output.Stdout);
        }

        [Fact]
        public void TestCreateReferenceImage()
        {
            Run("create_product", ProjectId, RegionName, ProductId, ProductDisplayName, ProductCategory);

            var output = Run("list_ref_images", ProjectId, RegionName, ProductId);
            Assert.DoesNotContain(RefImageId, output.Stdout);

            output = Run("create_ref_image", ProjectId, RegionName, ProductId, RefImageId, RefImageGcsUri);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_ref_images", ProjectId, RegionName, ProductId);
            Assert.Contains(RefImageId, output.Stdout);
        }


        [Fact]
        public void TestGetReferenceImage()
        {
            Run("create_product", ProjectId, RegionName, ProductId, ProductDisplayName, ProductCategory);
            Run("create_ref_image", ProjectId, RegionName, ProductId, RefImageId, RefImageGcsUri);

            var output = Run("get_ref_image", ProjectId, RegionName, ProductId, RefImageId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(RefImageId, output.Stdout);
        }

        [Fact]
        public void TestDeleteReferenceImage()
        {
            Run("create_product", ProjectId, RegionName, ProductId, ProductDisplayName, ProductCategory);
            Run("create_ref_image", ProjectId, RegionName, ProductId, RefImageId, RefImageGcsUri);

            var output = Run("list_ref_images", ProjectId, RegionName, ProductId);
            Assert.Contains(RefImageId, output.Stdout);

            output = Run("delete_ref_image", ProjectId, RegionName, ProductId, RefImageId);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_ref_images", ProjectId, RegionName, ProductId);
            Assert.DoesNotContain(RefImageId, output.Stdout);
        }

        [Fact]
        public void TestAddProductToSet()
        {
            Run("create_product", ProjectId, RegionName, ProductId, ProductDisplayName, ProductCategory);
            Run("create_product_set", ProjectId, RegionName, ProductSetId, ProductSetDisplayName);

            var output = Run("list_products_in_set", ProjectId, RegionName, ProductSetId);
            Assert.DoesNotContain(ProductId, output.Stdout);

            output = Run("add_product_to_set", ProjectId, RegionName, ProductId, ProductSetId);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_products_in_set", ProjectId, RegionName, ProductSetId);
            Assert.Contains(ProductId, output.Stdout);
        }

        [Fact]
        public void TestRemoveProductFromSet()
        {
            Run("create_product", ProjectId, RegionName, ProductId, ProductDisplayName, ProductCategory);
            Run("create_product_set", ProjectId, RegionName, ProductSetId, ProductSetDisplayName);
            Run("add_product_to_set", ProjectId, RegionName, ProductId, ProductSetId);

            var output = Run("list_products_in_set", ProjectId, RegionName, ProductSetId);
            Assert.Contains(ProductId, output.Stdout);

            output = Run("remove_product_from_set", ProjectId, RegionName, ProductId, ProductSetId);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_products_in_set", ProjectId, RegionName, ProductSetId);
            Assert.DoesNotContain(ProductId, output.Stdout);
        }

        [Fact]
        public void TestPurgeProductsInProductSet()
        {
            var output = Run("create_product", ProjectId, RegionName, ProductId, ProductDisplayName, ProductCategory);
            Assert.Equal(0, output.ExitCode);

            Run("create_product_set", ProjectId, RegionName, ProductSetId, ProductSetDisplayName);
            Run("add_product_to_set", ProjectId, RegionName, ProductId, ProductSetId);

            output = Run("list_products", ProjectId, RegionName);
            Assert.Contains(String.Format("Product id: {0}", ProductId), output.Stdout);

            Run("purge_products_in_product_set", ProjectId, RegionName, ProductSetId);
            output = Run("list_products", ProjectId, RegionName);
            Assert.DoesNotContain(String.Format("Product id: {0}", ProductId), output.Stdout);
        }

        [Fact]
        public void TestPurgeOrphanProducts()
        {
            Run("create_product", ProjectId, RegionName, ProductId, ProductDisplayName, ProductCategory);
            var output = Run("list_products", ProjectId, RegionName);
            Assert.Contains(String.Format("Product id: {0}", ProductId), output.Stdout);

            Run("purge_orphan_products", ProjectId, RegionName);

            output = Run("list_products", ProjectId, RegionName);
            Assert.DoesNotContain(String.Format("Product id: {0}", ProductId), output.Stdout);
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
            var output = Run("list_product_sets", ProjectId, RegionName);
            if (!output.Stdout.Contains(IndexedProductSet))
            {
                Run("create_product_set", ProjectId, RegionName, IndexedProductSet, ProductSetDisplayName);
            }

            output = Run("list_products", ProjectId, RegionName);
            if (!output.Stdout.Contains(IndexedProduct1))
            {
                Run("create_product", ProjectId, RegionName, IndexedProduct1, ProductDisplayName, ProductCategory);
            }

            output = Run("list_ref_images", ProjectId, RegionName, IndexedProduct1);
            if (!output.Stdout.Contains(RefImageId))
            {
                Run("create_ref_image", ProjectId, RegionName, IndexedProduct1, RefImageId, RefImageGcsUri);
            }

            output = Run("list_products_in_set", ProjectId, RegionName, IndexedProductSet);
            if (!output.Stdout.Contains(IndexedProduct1))
            {
                Run("add_product_to_set", ProjectId, RegionName, IndexedProduct1, IndexedProductSet);
            }

            output = Run("get_product", ProjectId, RegionName, IndexedProduct1);
            if (!output.Stdout.Contains("style") || !output.Stdout.Contains("womens"))
            {
                Run("update_product_labels", ProjectId, RegionName, IndexedProduct1, "style,womens");
            }
        }

        [Fact]
        public void TestProductSearch()
        {
            CreateProductSet();
            var output = Run("get_similar_products", ProjectId, RegionName, IndexedProductSet, ProductCategory, Path.Combine("data", ImageUri1), SearchFilter);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(IndexedProduct1, output.Stdout);
        }

        [Fact]
        public void TestProductSearchGcs()
        {
            CreateProductSet();
            var output = Run("get_similar_products_gcs", ProjectId, RegionName, IndexedProductSet, ProductCategory, RefImageGcsUri, SearchFilter);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(IndexedProduct1, output.Stdout);
        }

        [Fact]
        public void TestImportProductSets()
        {
            var output = Run("import_product_set", ProjectId, RegionName, CsvGcsUri);
            Assert.Equal(0, output.ExitCode);

            output = Run("list_product_sets", ProjectId, RegionName);
            Assert.Contains(ImportedProductSetId, output.Stdout);

            output = Run("list_products", ProjectId, RegionName);
            Assert.Contains(ImportedProductId, output.Stdout);
        }
    }
}
