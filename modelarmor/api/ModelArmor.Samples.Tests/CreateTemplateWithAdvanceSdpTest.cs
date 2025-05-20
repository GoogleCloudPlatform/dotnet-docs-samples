using System;
using Google.Cloud.ModelArmor.V1;
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class CreateTemplateWithAdvancedSdpTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateWithAdvancedSdpSample _sample;
        private readonly ITestOutputHelper _output;

        public CreateTemplateWithAdvancedSdpTests(
            ModelArmorFixture fixture,
            ITestOutputHelper output
        )
        {
            _fixture = fixture;
            _sample = new CreateTemplateWithAdvancedSdpSample();
            _output = output;
        }

        [Fact]
        public void CreateTemplateWithAdvancedSdpTest()
        {
            string inspectTemplateId = _fixture.InspectTemplateId;
            string deidentifyTemplateId = _fixture.DeidentifyTemplateId;

            string projectId = _fixture.ProjectId;
            string locationId = _fixture.LocationId;

            // Generate a unique template ID for testing
            string templateId = $"test-adv-sdp-{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            // Build the inspect template name.
            string inspectTemplateName =
                $"projects/{projectId}/locations/{locationId}/inspectTemplates/{inspectTemplateId}";

            // Build the deidentify template name.
            string deidentifyTemplateName =
                $"projects/{projectId}/locations/{locationId}/deidentifyTemplates/{deidentifyTemplateId}";

            _output.WriteLine($"Creating template with Advanced SDP: {templateId}");
            _output.WriteLine($"Using inspect template: {inspectTemplateId}");
            _output.WriteLine($"Using deidentify template: {deidentifyTemplateId}");

            // Run the sample
            Template template = _sample.CreateTemplateWithAdvancedSdp(
                projectId: projectId,
                locationId: locationId,
                templateId: templateId,
                inspectTemplateId: inspectTemplateId,
                deidentifyTemplateId: deidentifyTemplateId
            );

            // Output template details
            _output.WriteLine($"Created template: {template.Name}");

            // Verify the template was created successfully
            Assert.NotNull(template);
            Assert.Contains(templateId, template.Name);

            // Verify the template has the expected filter configuration
            Assert.NotNull(template.FilterConfig);
            Assert.NotNull(template.FilterConfig.SdpSettings);
            Assert.NotNull(template.FilterConfig.SdpSettings.AdvancedConfig);

            // Verify the advanced SDP config
            var advancedConfig = template.FilterConfig.SdpSettings.AdvancedConfig;
            Assert.Contains(inspectTemplateId, advancedConfig.InspectTemplate);
            Assert.Contains(deidentifyTemplateId, advancedConfig.DeidentifyTemplate);

            // Verify the inspect and deidentify template names
            Assert.Equal(inspectTemplateName, advancedConfig.InspectTemplate);
            Assert.Equal(deidentifyTemplateName, advancedConfig.DeidentifyTemplate);

            // Clean up - delete the template
            try
            {
                _fixture.Client.DeleteTemplate(new DeleteTemplateRequest { Name = template.Name });
                _output.WriteLine($"Deleted template: {template.Name}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error during cleanup: {ex.Message}");
                // Don't fail the test if cleanup fails
            }
        }
    }
}
