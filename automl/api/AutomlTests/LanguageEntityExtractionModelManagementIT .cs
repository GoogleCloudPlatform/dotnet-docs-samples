using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class LanguageEntityExtractionModelManagementIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        public LanguageEntityExtractionModelManagementIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _modelId = "TEN623594616762925056";
        }

        [Fact]
        public void TestDeployUndeployModel()
        {
            // export dataset
            ConsoleOutput output = _fixture.SampleRunner.Run("undeploy_model", _fixture.ProjectId, _modelId);

            Assert.Contains("Model undeployment finished", output.Stdout);

            output = _fixture.SampleRunner.Run("deploy_model", _fixture.ProjectId, _modelId);

            Assert.Contains("Model deployment finished", output.Stdout);
        }
    }
}
