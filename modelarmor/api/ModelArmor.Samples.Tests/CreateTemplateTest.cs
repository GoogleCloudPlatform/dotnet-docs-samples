using Google.Cloud.ModelArmor.V1;
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class CreateTemplateTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateSample _sample;
        private readonly ITestOutputHelper _output;

        public CreateTemplateTests(ModelArmorFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _sample = new CreateTemplateSample();
            _output = output;
        }

        [Fact]
        public void Runs()
        {
            // Generate a unique template ID for testing
            string templateId =
                $"test-template-{System.Guid.NewGuid().ToString("N").Substring(0, 8)}";

            // Run the sample
            Template template = _sample.CreateTemplate(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateId
            );

            // Verify the template was created successfully
            Assert.NotNull(template);
            Assert.Contains(templateId, template.Name);

            // Verify the template has the expected filter configuration
            Assert.NotNull(template.FilterConfig);
            Assert.NotNull(template.FilterConfig.RaiSettings);

            // Verify that the RAI filters were created correctly
            var raiFilters = template.FilterConfig.RaiSettings.RaiFilters;
            Assert.Equal(4, raiFilters.Count);

            // Verify specific filters - adjust based on your implementation
            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.Dangerous
                    && f.ConfidenceLevel == DetectionConfidenceLevel.High
            );
            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.HateSpeech
                    && f.ConfidenceLevel == DetectionConfidenceLevel.MediumAndAbove
            );
            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.SexuallyExplicit
                    && f.ConfidenceLevel == DetectionConfidenceLevel.MediumAndAbove
            );
            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.Harassment
                    && f.ConfidenceLevel == DetectionConfidenceLevel.MediumAndAbove
            );

            // Clean up - delete the template
            try
            {
                _fixture.Client.DeleteTemplate(new DeleteTemplateRequest { Name = template.Name });
                _output.WriteLine($"Deleted template: {template.Name}");
            }
            catch (System.Exception ex)
            {
                _output.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }
    }
}
