using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class ProductSearchTest : IDisposable
    {
        private const string REGION_NAME = "us-west1";
        private const string PRODUCT_DISPLAY_NAME = "fake_product_display_name_for_testing";
        private const string PRODUCT_CATEGORY = "apparel";
        private const string PRODUCT_SET_DISPLAY_NAME = "fake_product_set_display_name_for_testing";
        private const string REF_IMAGE_GCS_URI = "gs://cloud-samples-data/vision/product_search/shoes_1.jpg";
        private const string CSV_GCS_URI = "gs://cloud-samples-data/vision/product_search/product_sets.csv";
        private const string IMAGE_URI_1 = "shoes_1.jpg";
        private const string IMAGE_URI_2 = "shoes_2.jpg";
        private const string SEARCH_FILTER = "style=womens";

        private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        private readonly Stack<string[]> _cleanupOperations = new Stack<string[]>();

        readonly CommandLineRunner _productSearch = new CommandLineRunner()
        {
            Main = ProductSearchProgram.Main,
            Command = "Product Search"
        };

        string NewProductId() => ScopedProductId(
            "fake_productId_" + TestUtil.RandomName());

        string ScopedProductId(string productId) 
        {
            _cleanupOperations.Push(new []{"delete_product", _projectId, REGION_NAME, productId});
            return productId;
        }

        string NewProductSetId() => ScopedProductSetId("fake_productSetId_" + TestUtil.RandomName());

        string ScopedProductSetId(string productSetId)
        {
            _cleanupOperations.Push(new []{"delete_product_set", _projectId, REGION_NAME, productSetId});
            return productSetId;
        }

        string NewRefImageId(string productId) 
        {
            string refImageId = "fake_ref_image_id_" + TestUtil.RandomName();
            _cleanupOperations.Push(new []{"delete_ref_image", _projectId, REGION_NAME, productId, refImageId});
            return refImageId;
        }

        public void Dispose()
        {
            string[] cleanupOperation;
            while (_cleanupOperations.TryPop(out cleanupOperation)) 
            {
                _productSearch.Run(cleanupOperation);
            }
        }

        [Fact]
        public void TestCreateProduct()
        {
            string productId = NewProductId();
            var output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(productId, output.Stdout);

            output = _productSearch.Run("create_product", _projectId, REGION_NAME, productId, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(productId, output.Stdout);
        }

        [Fact]
        public void TestGetProduct()
        {
            string productId = NewProductId();
            _productSearch.Run("create_product", _projectId, REGION_NAME, productId, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = _productSearch.Run("get_product", _projectId, REGION_NAME, productId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(_projectId, output.Stdout);
        }

        [Fact]
        public void TestDeleteProduct()
        {
            string productId = NewProductId();
            _productSearch.Run("create_product", _projectId, REGION_NAME, productId, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            var output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(productId, output.Stdout);

            output = _productSearch.Run("delete_product", _projectId, REGION_NAME, productId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Product deleted.", output.Stdout);

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(productId, output.Stdout);
        }

        [Fact]
        public void TestCreateProductSet()
        {
            string productSetId = NewProductSetId();
            var output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.DoesNotContain(productSetId, output.Stdout);

            output = _productSearch.Run("create_product_set", _projectId, REGION_NAME, productSetId, PRODUCT_SET_DISPLAY_NAME);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(PRODUCT_SET_DISPLAY_NAME, output.Stdout);

            output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.Contains(productSetId, output.Stdout);
        }


        [Fact]
        public void TestGetProductSet()
        {
            string productSetId = NewProductSetId();
            _productSearch.Run("create_product_set", _projectId, REGION_NAME, productSetId, PRODUCT_SET_DISPLAY_NAME);

            var output = _productSearch.Run("get_product_set", _projectId, REGION_NAME, productSetId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(productSetId, output.Stdout);
        }

        [Fact]
        public void TestDeleteProductSet()
        {
            string productSetId = NewProductSetId();
            _productSearch.Run("create_product_set", _projectId, REGION_NAME, productSetId, PRODUCT_SET_DISPLAY_NAME);

            var output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.Contains(productSetId, output.Stdout);

            output = _productSearch.Run("delete_product_set", _projectId, REGION_NAME, productSetId);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.DoesNotContain(productSetId, output.Stdout);
        }

        [Fact]
        public void TestCreateReferenceImage()
        {
            string productId = NewProductId();
            _productSearch.Run("create_product", _projectId, REGION_NAME, productId, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);

            string refImageId = NewRefImageId(productId);
            var output = _productSearch.Run("list_ref_images", _projectId, REGION_NAME, productId);
            Assert.DoesNotContain(refImageId, output.Stdout);

            output = _productSearch.Run("create_ref_image", _projectId, REGION_NAME, productId, refImageId, REF_IMAGE_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_ref_images", _projectId, REGION_NAME, productId);
            Assert.Contains(refImageId, output.Stdout);
        }


        [Fact]
        public void TestGetReferenceImage()
        {
            string productId = NewProductId();
            _productSearch.Run("create_product", _projectId, REGION_NAME, productId, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            string refImageId = NewRefImageId(productId);
            _productSearch.Run("create_ref_image", _projectId, REGION_NAME, productId, refImageId, REF_IMAGE_GCS_URI);

            var output = _productSearch.Run("get_ref_image", _projectId, REGION_NAME, productId, refImageId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(refImageId, output.Stdout);
        }

        [Fact]
        public void TestDeleteReferenceImage()
        {
            string productId = NewProductId();
            _productSearch.Run("create_product", _projectId, REGION_NAME, productId, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            string REF_IMAGE_ID = NewRefImageId(productId);
            _productSearch.Run("create_ref_image", _projectId, REGION_NAME, productId, REF_IMAGE_ID, REF_IMAGE_GCS_URI);

            var output = _productSearch.Run("list_ref_images", _projectId, REGION_NAME, productId);
            Assert.Contains(REF_IMAGE_ID, output.Stdout);

            output = _productSearch.Run("delete_ref_image", _projectId, REGION_NAME, productId, REF_IMAGE_ID);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_ref_images", _projectId, REGION_NAME, productId);
            Assert.DoesNotContain(REF_IMAGE_ID, output.Stdout);
        }

        [Fact]
        public void TestImportProductSets()
        {
            string productSetId = ScopedProductSetId("fake_productSetId_for_testing");
            var output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.DoesNotContain(productSetId, output.Stdout);

            string productId = ScopedProductId("fake_productId_for_testing_1");
            string productId2 = ScopedProductId("fake_productId_for_testing_2");
            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.DoesNotContain(productId, output.Stdout);
            Assert.DoesNotContain(productId2, output.Stdout);

            output = _productSearch.Run("import_product_set", _projectId, REGION_NAME, CSV_GCS_URI);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_product_sets", _projectId, REGION_NAME);
            Assert.Contains(productSetId, output.Stdout);

            output = _productSearch.Run("list_products", _projectId, REGION_NAME);
            Assert.Contains(productId, output.Stdout);
            Assert.Contains(productId2, output.Stdout);
        }

        [Fact]
        public void TestAddProductToSet()
        {
            string productId = NewProductId();
            _productSearch.Run("create_product", _projectId, REGION_NAME, productId, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            string productSetId = NewProductSetId();
            _productSearch.Run("create_product_set", _projectId, REGION_NAME, productSetId, PRODUCT_SET_DISPLAY_NAME);

            var output = _productSearch.Run("list_products_in_set", _projectId, REGION_NAME, productSetId);
            Assert.DoesNotContain(productId, output.Stdout);

            output = _productSearch.Run("add_product_to_set", _projectId, REGION_NAME, productId, productSetId);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products_in_set", _projectId, REGION_NAME, productSetId);
            Assert.Contains(productId, output.Stdout);
        }

        [Fact]
        public void TestRemoveProductFromSet()
        {
            string productId = NewProductId();
            _productSearch.Run("create_product", _projectId, REGION_NAME, productId, PRODUCT_DISPLAY_NAME, PRODUCT_CATEGORY);
            string productSetId = NewProductSetId();
            _productSearch.Run("create_product_set", _projectId, REGION_NAME, productSetId, PRODUCT_SET_DISPLAY_NAME);
            _productSearch.Run("add_product_to_set", _projectId, REGION_NAME, productId, productSetId);

            var output = _productSearch.Run("list_products_in_set", _projectId, REGION_NAME, productSetId);
            Assert.Contains(productId, output.Stdout);

            output = _productSearch.Run("remove_product_from_set", _projectId, REGION_NAME, productId, productSetId);
            Assert.Equal(0, output.ExitCode);

            output = _productSearch.Run("list_products_in_set", _projectId, REGION_NAME, productSetId);
            Assert.DoesNotContain(productId, output.Stdout);
        }

        [Fact]
        public void TestProductSearch()
        {
            string INDEXED_PRODUCT_SET = NewProductSetId();
            string INDEXED_PRODUCT_1 = NewProductId();
            var output = _productSearch.Run("get_similar_products", _projectId, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_CATEGORY, Path.Combine("data", IMAGE_URI_1), SEARCH_FILTER);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(INDEXED_PRODUCT_1, output.Stdout);
        }

        [Fact]
        public void TestProductSearchGcs()
        {
            string INDEXED_PRODUCT_SET = NewProductSetId();
            string INDEXED_PRODUCT_1 = NewProductId();
            var output = _productSearch.Run("get_similar_products_gcs", _projectId, REGION_NAME, INDEXED_PRODUCT_SET, PRODUCT_CATEGORY, REF_IMAGE_GCS_URI, SEARCH_FILTER);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(INDEXED_PRODUCT_1, output.Stdout);
        }
    }
}
