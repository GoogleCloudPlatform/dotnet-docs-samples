using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class LanguageEntityExtractionPredictIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        public LanguageEntityExtractionPredictIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _modelId = "TEN623594616762925056";
        }

        [Fact]
        public void TestPredict()
        {
            string text = "Constitutional mutations in the WT1 gene in patients with Denys-Drash syndrome.";
            ConsoleOutput output = _fixture.SampleRunner.Run("language_entity_extraction_predict",
_fixture.ProjectId, _modelId, text);

            // Assert
            Assert.Contains("Text Extract Entity Type:", output.Stdout);
        }

        [Fact]
        public void TestBatchPredict()
        {
            string bucketId = $"{ _fixture.ProjectId}-lcm";
            string inputUri = $"gs://{bucketId}/entity_extraction/input.jsonl";
            string outputUri = $"gs://{_fixture.Bucket.Name}/TEST_BATCH_PREDICT/";

            // Act
            ConsoleOutput output = _fixture.SampleRunner.Run("language_batch_predict",
_fixture.ProjectId, _modelId, inputUri, outputUri);

            // Assert
            Assert.Contains("Batch Prediction results saved to specified Cloud Storage bucket", output.Stdout);
        }
    }
}
