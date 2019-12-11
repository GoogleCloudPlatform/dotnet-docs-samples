using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class LanguageEntityExtractionDatasetManagementIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _datasetId;
        public LanguageEntityExtractionDatasetManagementIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _datasetId = "TEN3278932790028009472";
        }

        [Fact]
        public void TestLangEntityExtractCreateImportDeleteDataset()
        {
            // Create a random dataset name with a length of 32 characters (max allowed by AutoML)
            // To prevent name collisions when running tests in multiple java versions at once.
            // AutoML doesn't allow "-", but accepts "_"
            string datasetName = "test_dataset_" + TestUtil.RandomName();
            datasetName = datasetName.Substring(0, 32);

            // create dataset
            ConsoleOutput output = _fixture.SampleRunner.Run("create_dataset_language_entity_extraction",
            _fixture.ProjectId, datasetName);

            Assert.Contains("Dataset name: ", output.Stdout);

            // import data
            string bucket = $"gs://{ _fixture.ProjectId}-lcm";

            string datasetId = output.Stdout.Split("\n")[1].Split()[2];
            string data = bucket + "/entity_extraction/dataset.csv";
            output = _fixture.SampleRunner.Run("import_dataset",
            _fixture.ProjectId, datasetId, data);

            Assert.Contains("Data imported.", output.Stdout);

            // delete dataset
            output = _fixture.SampleRunner.Run("delete_dataset",
            _fixture.ProjectId, datasetId);

            Assert.Contains("Dataset deleted", output.Stdout);
        }

        [Fact]
        public void TestListDataset()
        {
            // list datasets
            ConsoleOutput output = _fixture.SampleRunner.Run("list_datasets", _fixture.ProjectId);

            Assert.Contains("Dataset id:", output.Stdout);
        }

        [Fact]
        public void TestGetDataset()
        {
            // get dataset
            ConsoleOutput output = _fixture.SampleRunner.Run("get_dataset", _fixture.ProjectId, _datasetId);

            Assert.Contains("Dataset id:", output.Stdout);
        }

        [Fact]
        public void TestExportDataset()
        {
            // export dataset
            ConsoleOutput output = _fixture.SampleRunner.Run("export_dataset", _fixture.ProjectId, _datasetId,
              $"gs://{ _fixture.Bucket.Name}/TEST_EXPORT_OUTPUT/");

            Assert.Contains("Dataset exported.", output.Stdout);
        }
    }
}
