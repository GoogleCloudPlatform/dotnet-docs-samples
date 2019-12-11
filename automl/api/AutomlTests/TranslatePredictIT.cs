using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class TranslatePredictIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId = "TRL8772189639420149760";
        private readonly string _filePath = "./resources/input.txt";
        public TranslatePredictIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestPredictTranslation()
        {
            // Act
            ConsoleOutput output = _fixture.SampleRunner.Run("translate_predict",
_fixture.ProjectId, _modelId, _filePath);

            // Assert
            Assert.Contains("Translated Content:", output.Stdout);
        }
    }
}
