using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class VisionClassificationDatasetManagementIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _datasetId = "TST6012617763841900544";
        public VisionClassificationDatasetManagementIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestVisionClassificationCreateImportDeleteDataset()
        {
            string datasetName = "test_dataset_" + TestUtil.RandomName();
            datasetName = datasetName.Substring(0, 32);

            // create dataset
            ConsoleOutput output = _fixture.SampleRunner.Run("create_dataset_vision_classification",
            _fixture.ProjectId, datasetName);

            Assert.Contains("Dataset name: ", output.Stdout);

            // import data
            string datasetId = output.Stdout.Split("\n")[1].Split()[2];
            string data = $"gs://{_fixture.ProjectId}-vcm/flower_traindata.csv";
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
