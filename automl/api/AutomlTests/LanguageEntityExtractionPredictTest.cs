using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class LanguageEntityExtractionPredictTest
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        public LanguageEntityExtractionPredictTest(AutoMLFixture fixture)
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
    }
}
