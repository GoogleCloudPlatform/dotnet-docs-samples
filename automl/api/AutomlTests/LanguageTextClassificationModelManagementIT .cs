using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class LanguageTextClassificationModelManagementIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        public LanguageTextClassificationModelManagementIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _modelId = "TCN6081675890159255552";
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
