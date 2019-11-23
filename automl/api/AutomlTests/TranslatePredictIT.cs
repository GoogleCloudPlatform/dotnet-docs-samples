using Grpc.Core;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class TranslatePredictIT
    {
        private readonly AutoMLFixture _fixture;
        private string _modelId = "TRL3128559826197068699";
        private string _filePath = "./resources/input.txt";
        public TranslatePredictIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void testPredict()
        {
            // Act
            ConsoleOutput output = _fixture.SampleRunner.Run("translate_predict",
_fixture.ProjectId, _modelId, _filePath);

            // Assert
            Assert.Contains("Translated Content:", output.Stdout);
        }
    }
}
