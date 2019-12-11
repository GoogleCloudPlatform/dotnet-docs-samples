using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class LanguageSentimentAnalysisPredictIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        public LanguageSentimentAnalysisPredictIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _modelId = "TST3802432266244718592";
        }

        [Fact]
        public void TestPredict()
        {
            string text = "Hopefully this Claritin kicks in soon";
            ConsoleOutput output = _fixture.SampleRunner.Run("language_entity_extraction_predict",
_fixture.ProjectId, _modelId, text);

            Assert.Contains("Predicted sentiment score:", output.Stdout);
        }
    }
}
