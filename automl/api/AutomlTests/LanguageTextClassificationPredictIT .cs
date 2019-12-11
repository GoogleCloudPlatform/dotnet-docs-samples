using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class LanguageTextClassificationPredictIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        public LanguageTextClassificationPredictIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _modelId = "TCN6081675890159255552";
        }

        [Fact]
        public void TestPredict()
        {
            string text = "Fruit and nut flavour";
            ConsoleOutput output = _fixture.SampleRunner.Run("language_entity_extraction_predict",
_fixture.ProjectId, _modelId, text);

            Assert.Contains("Predicted class name:", output.Stdout);
        }
    }
}
